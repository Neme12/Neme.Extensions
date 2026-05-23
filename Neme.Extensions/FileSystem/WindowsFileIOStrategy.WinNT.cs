using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Windows.Wdk.Foundation;
using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

[SupportedOSPlatform("windows6.0.6000")]
internal sealed partial class WindowsFileIOStrategy
{
    // Cache of volume serial numbers (full 64-bit) to volume handles
    // This avoids repeatedly enumerating all volumes for file ID operations
    private static readonly ConcurrentDictionary<ulong, SafeFileHandle> s_volumeHandleCache = new();

    [return: OwnershipTransfer]
    public override unsafe CustomSafeFileHandle OpenHandle(FsFileId fileId, FsFileOptions options)
    {
        ValidateFileId(fileId);

        FileIOEventSource.Log.OpeningFileById(fileId.VolumeSerialNumber, fileId.FileIdLow, fileId.FileIdHigh);

        // Find and open the volume with the matching serial number
        using var volumeHandle = FindAndOpenVolumeBySerialNumber(fileId.VolumeSerialNumber).CreateScope();

        var fileIdBuffer = stackalloc ulong[2];
        fileIdBuffer[0] = fileId.FileIdLow;
        fileIdBuffer[1] = fileId.FileIdHigh;

        UNICODE_STRING unicodeString = new()
        {
            Length = 16,
            MaximumLength = 16,
            Buffer = (char*)fileIdBuffer,
        };

        var objectAttributes = new OBJECT_ATTRIBUTES
        {
            Length = (uint)sizeof(OBJECT_ATTRIBUTES),
            RootDirectory = new HANDLE(volumeHandle.Handle),
            ObjectName = &unicodeString,
        };

        var createOptions =
            options.Options.ToWinNT(options.Attributes) |
            NTCREATEFILE_CREATE_OPTIONS.FILE_OPEN_BY_FILE_ID;

        var status = WinNTPInvoke.NtCreateFile(
            out var handle,
            options.Access.ToWin32(),
            in objectAttributes,
            out _,
            null,
            options.Attributes.ToWinNT(),
            options.Share.ToWin32(),
            options.Mode.ToWinNT(),
            createOptions,
            []);

        if (status.SeverityCode != NTSTATUS.Severity.Success)
            throw WinNtMarshal.GetExceptionForNtStatus(status);

        FileIOEventSource.Log.FileOpenedById(fileId.VolumeSerialNumber, fileId.FileIdLow, fileId.FileIdHigh);

        return new CustomSafeFileHandle(handle, ownsHandle: true);
    }

    [return: OwnershipTransfer]
    public override unsafe CustomSafeFileHandle OpenHandleBy([Borrow] SafeFileHandle? rootDirectory, string? path, FsFileOptions options)
#pragma warning disable RS0042
    {
        if (rootDirectory is null && path is null)
            throw new ArgumentException($"Either {nameof(rootDirectory)} or {nameof(path)} must be provided.");

        ValidateFileHandle(rootDirectory, optional: true);
        ValidatePath(path, optional: true);

        UNICODE_STRING unicodeString = default;

        if (path is not null)
        {
            if (rootDirectory is null)
                path = @"\??\" + Path.GetFullPath(path);
        }

        NTSTATUS status;
        HANDLE handle;

        fixed (char* pathRef = path)
        {
            if (path is not null)
                Win32PInvoke.RtlInitUnicodeString(&unicodeString, (PCWSTR)pathRef);

            using var rootDirectoryHandle = rootDirectory?.CreateScope();

            var objectAttributes = new OBJECT_ATTRIBUTES
            {
                Length = (uint)sizeof(OBJECT_ATTRIBUTES),
                RootDirectory = rootDirectoryHandle is not null
                    ? new(rootDirectoryHandle.Value.Handle)
                    : HANDLE.Null,
                ObjectName = &unicodeString,
                Attributes = OBJECT_ATTRIBUTE_FLAGS.OBJ_CASE_INSENSITIVE,
            };

            status = WinNTPInvoke.NtCreateFile(
                out handle,
                options.Access.ToWin32(),
                in objectAttributes,
                out _,
                null,
                options.Attributes.ToWinNT(),
                options.Share.ToWin32(),
                options.Mode.ToWinNT(),
                options.Options.ToWinNT(options.Attributes),
                []);
        }

        if (status.SeverityCode != NTSTATUS.Severity.Success)
            throw WinNtMarshal.GetExceptionForNtStatus(status, path);

        return new CustomSafeFileHandle(handle, ownsHandle: true);
    }
#pragma warning restore RS0042

    private static SafeFileHandle FindAndOpenVolumeBySerialNumber(ulong volumeSerialNumber)
    {
        // Use GetOrAdd for atomic cache lookup/creation - ensures only one thread enumerates volumes
        // for a given serial number, even under concurrent access
        return s_volumeHandleCache.GetOrAdd(volumeSerialNumber, static serial =>
        {
            FileIOEventSource.Log.VolumeHandleCacheMiss(serial);
            var handle = EnumerateAndOpenVolume(serial);
            FileIOEventSource.Log.VolumeHandleCached(serial);
            return handle;
        });
    }

    private static unsafe SafeFileHandle EnumerateAndOpenVolume(ulong targetSerial)
    {
        FileIOEventSource.Log.EnumeratingVolumes(targetSerial);

        char* volumeNameBuffer = stackalloc char[50]; // Volume GUID paths are typically 49 chars
        Span<char> volumeName = new(volumeNameBuffer, 50);

        var findHandle = Win32PInvoke.FindFirstVolume(volumeName);
        if (findHandle.IsInvalid)
            throw Win32Marshal.GetExceptionForLastWin32Error();

        try
        {
            do
            {
                // Remove trailing backslash for GetVolumeInformation
                var volumePathLength = volumeName.IndexOf('\0');
                if (volumePathLength > 0 && volumeName[volumePathLength - 1] == '\\')
                    volumePathLength--;

                var volumePath = volumeName[..volumePathLength];

                // Get volume information to check serial number (lower 32 bits only)
                fixed (char* pVolumePath = volumePath)
                {
                    uint serialNumberLower32 = default;

                    if (Win32PInvoke.GetVolumeInformation(
                        new PCWSTR(pVolumePath),
                        null,
                        0,
                        &serialNumberLower32,
                        null,
                        null,
                        null,
                        0))
                    {
                        // Lower 32 bits match - try to open and verify full 64-bit serial
                        if (serialNumberLower32 == (uint)targetSerial)
                        {
                            FileIOEventSource.Log.VolumePartialMatch(serialNumberLower32);
                            var volumePathWithSlash = volumeName[..(volumePathLength + 1)].ToString();
                            if (TryOpenAndVerifyVolume(volumePathWithSlash, targetSerial, out var handle))
                                return handle;
                        }
                    }
                    else
                    {
                        // GetVolumeInformation failed - volume might not be accessible
                        var error = new Win32Exception();
                        FileIOEventSource.Log.VolumeInformationFailed(volumePath.ToString(), error.NativeErrorCode, error.Message);
                    }
                }
            }
            while (Win32PInvoke.FindNextVolume((HANDLE)findHandle.DangerousGetHandle(), volumeName));
        }
        finally
        {
            Win32PInvoke.FindVolumeClose(findHandle);
        }

        FileIOEventSource.Log.VolumeNotFound(targetSerial);
        throw new DirectoryNotFoundException($"No volume found with serial number 0x{targetSerial:X8}");
    }

    private static unsafe bool TryOpenAndVerifyVolume(
        string volumePath,
        ulong targetSerial,
        [NotNullWhen(true)] out SafeFileHandle? handle)
    {
        var volumeHandle = OwnedOrBorrowed.CreateOwned(Win32PInvoke.CreateFile(
            volumePath,
            0, // No specific access needed, just need the handle
            FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
            null,
            FILE_CREATION_DISPOSITION.OPEN_EXISTING,
            FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS,
            null));

        if (volumeHandle.Value.IsInvalid)
        {
            var error = new Win32Exception();
            FileIOEventSource.Log.VolumeHandleOpenFailed(volumePath, error.NativeErrorCode, error.Message);
            handle = null;
            return false;
        }

        using var handleScope = volumeHandle.Value.CreateScope();

        // Get full 64-bit volume serial number to verify
        FILE_ID_INFO fileIdInfo = default;
        if (Win32PInvoke.GetFileInformationByHandleEx(
            new HANDLE(handleScope.Handle),
            FILE_INFO_BY_HANDLE_CLASS.FileIdInfo,
            &fileIdInfo,
            (uint)sizeof(FILE_ID_INFO)))
        {
            // Compare the full 64-bit serial number
            if (fileIdInfo.VolumeSerialNumber == targetSerial)
            {
                // Full match confirmed
                FileIOEventSource.Log.VolumeVerified(targetSerial);
                handle = volumeHandle.Move();
                return true;
            }
            else
            {
                // Serial number mismatch
                FileIOEventSource.Log.VolumeSerialMismatch(targetSerial, fileIdInfo.VolumeSerialNumber);
            }
        }
        else
        {
            // Failed to get file information
            var error = new Win32Exception();
            FileIOEventSource.Log.GetFileInformationFailed(volumePath, error.NativeErrorCode, error.Message);
        }

        // Not a match or couldn't get info - close handle and return false
        handle = null;
        return false;
    }
}

using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Windows.Wdk.Foundation;
using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static SafeFileHandle ReopenHandle([Borrow] SafeFileHandle file, FsFileOptions options)
    {
        ValidateFileHandle(file);

        return OpenHandleBy(file, null, options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static FsFile Reopen([Borrow] FsFile file)
    {
        ValidateFileHandle(file.Handle);

        return new(OpenHandleBy(file.Handle, null, file.Options), file.Options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static unsafe SafeFileHandle OpenHandle(
        FsFileId fileId,
        FsFileOptions options)
    {
        ValidateFileId(fileId);

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

        return new SafeFileHandle(handle, ownsHandle: true);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static FsFile Open(
        FsFileId fileId,
        FsFileOptions options)
    {
        return new(OpenHandle(fileId, options), options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    public static bool TryOpenHandle(
        FsFileId fileId,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
        bool requireDirectory = true)
    {
        try
        {
            handle = OpenHandle(fileId, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            handle = null;
            return false;
        }
    }

    [SupportedOSPlatform("windows5.1.2600")]
    public static bool TryOpen(
        FsFileId fileId,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        try
        {
            file = Open(fileId, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [SupportedOSPlatform("windows5.1.2600")]
    private static unsafe SafeFileHandle FindAndOpenVolumeBySerialNumber(ulong volumeSerialNumber)
    {
        // Note: GetVolumeInformation only returns the lower 32 bits of the volume serial number.
        // FILE_ID_INFO.VolumeSerialNumber is 64-bit, but we compare only the lower 32 bits.
        var targetSerial = (uint)volumeSerialNumber;

        char* volumeNameBuffer = stackalloc char[50]; // Volume GUID paths are typically 49 chars
        Span<char> volumeName = new(volumeNameBuffer, 50);

        var findHandle = Win32PInvoke.FindFirstVolume(volumeName);
        if (findHandle.IsInvalid)
            throw Win32Marshal.GetExceptionForLastWin32Error("Failed to enumerate volumes");

        try
        {
            do
            {
                // Remove trailing backslash for GetVolumeInformation
                var volumePathLength = volumeName.IndexOf('\0');
                if (volumePathLength > 0 && volumeName[volumePathLength - 1] == '\\')
                    volumePathLength--;

                var volumePath = volumeName[..volumePathLength];

                try
                {
                    // Get volume information to check serial number
                    fixed (char* pVolumePath = volumePath)
                    {
                        uint serialNumber = default;

                        if (Win32PInvoke.GetVolumeInformation(
                            new PCWSTR(pVolumePath),
                            null,
                            0,
                            &serialNumber,
                            null,
                            null,
                            null,
                            0))
                        {
                            if (serialNumber == targetSerial)
                            {
                                // Found the matching volume, open a handle to it
                                var volumePathWithSlash = volumeName[..(volumePathLength + 1)].ToString();

                                var handle = Win32PInvoke.CreateFile(
                                    volumePathWithSlash,
                                    0, // No specific access needed, just need the handle
                                    FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                                    null,
                                    FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                                    FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS,
                                    null);

                                if (handle.IsInvalid)
                                    throw Win32Marshal.GetExceptionForLastWin32Error($"Failed to open volume {volumePathWithSlash}");

                                return new SafeFileHandle(handle.DangerousGetHandle(), ownsHandle: true);
                            }
                        }
                    }
                }
                catch
                {
                    // Volume might not be accessible, continue to next
                }
            }
            while (Win32PInvoke.FindNextVolume((HANDLE)findHandle.DangerousGetHandle(), volumeName));
        }
        finally
        {
            Win32PInvoke.FindVolumeClose(findHandle);
        }

        throw new DirectoryNotFoundException($"No volume found with serial number 0x{volumeSerialNumber:X16}");
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static unsafe SafeFileHandle OpenHandleBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
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

            Win32PInvoke.RtlInitUnicodeString(ref unicodeString, path);
        }

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

        var status = WinNTPInvoke.NtCreateFile(
            out var handle,
            options.Access.ToWin32(),
            in objectAttributes,
            out _,
            null,
            options.Attributes.ToWinNT(),
            options.Share.ToWin32(),
            options.Mode.ToWinNT(),
            options.Options.ToWinNT(options.Attributes),
            []);

        if (status.SeverityCode != NTSTATUS.Severity.Success)
            throw WinNtMarshal.GetExceptionForNtStatus(status, path);

        return new SafeFileHandle(handle, ownsHandle: true);
    }
#pragma warning restore RS0042

    [SupportedOSPlatform("windows5.1.2600")]
    public static bool TryOpenHandleBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? file,
        bool requireDirectory = true)
    {
        try
        {
            file = OpenHandleBy(rootDirectory, path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static unsafe FsFile OpenBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
    {
        return new(OpenHandleBy(rootDirectory, path, options), options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static bool TryOpenBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        try
        {
            file = OpenBy(rootDirectory, path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }
}

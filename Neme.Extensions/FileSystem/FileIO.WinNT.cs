using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.ComponentModel;
using System.Runtime.Versioning;
using Windows.Wdk.Foundation;
using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Foundation;
using Win32PInvoke = Windows.Win32.PInvoke;
using WinNTPInvoke = Windows.Wdk.PInvoke;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static SafeFileHandle Reopen(SafeFileHandle file, FsFileOptions options)
    {
        ValidateFileHandle(file);

        return Open(file, null, options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static unsafe SafeFileHandle Open(
        SafeFileHandle? rootDirectoory,
        string? path,
        FsFileOptions options)
    {
        if (rootDirectoory is null && path is null)
            throw new ArgumentException($"Either {nameof(rootDirectoory)} or {nameof(path)} must be provided.");
        if (rootDirectoory is not null)
            ValidateFileHandle(rootDirectoory);
        if (path is not null)
            ValidatePath(path);

        UNICODE_STRING unicodeString = default;

        if (path is not null)
        {
            path = @"\??\" + path;
            Win32PInvoke.RtlInitUnicodeString(ref unicodeString, path);
        }

        using var rootDirectoryHandle = rootDirectoory?.CreateScope();

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
            0,
            options.Attributes.ToWin32(),
            options.Share.ToWin32(),
            options.Mode.ToWinNT(),
            options.Options.ToWinNT() | NTCREATEFILE_CREATE_OPTIONS.FILE_NON_DIRECTORY_FILE,
            []);

        if (status.SeverityCode != NTSTATUS.Severity.Success)
        {
            var win32Error = Win32PInvoke.RtlNtStatusToDosError(status);
            var win32Exception = new Win32Exception((int)win32Error);
            throw Win32Marshal.GetExceptionForWin32Error(win32Exception, path);
        }

        return new SafeFileHandle(handle, ownsHandle: true);
    }
}

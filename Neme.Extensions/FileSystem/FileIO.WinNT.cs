using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.Runtime.Versioning;
using Windows.Wdk.Foundation;
using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Foundation;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static SafeFileHandle Reopen([Borrow] SafeFileHandle file, FsFileOptions options)
    {
        ValidateFileHandle(file);

        return OpenBy(file, null, options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static CheckedFileStream ReopenFileStream([Borrow] SafeFileHandle file, FsFileOptions options)
    {
        ValidateFileHandle(file);

        return OpenFileStreamBy(file, null, options);
    }

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static unsafe SafeFileHandle OpenBy(
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
            0,
            options.Attributes.ToWin32(),
            options.Share.ToWin32(),
            options.Mode.ToWinNT(),
            options.Options.ToWinNT() | NTCREATEFILE_CREATE_OPTIONS.FILE_NON_DIRECTORY_FILE,
            []);

        if (status.SeverityCode != NTSTATUS.Severity.Success)
            throw WinNtMarshal.GetExceptionForNtStatus(status, path);

        return new SafeFileHandle(handle, ownsHandle: true);
    }
#pragma warning restore RS0042

    [SupportedOSPlatform("windows5.1.2600")]
    [return: OwnershipTransfer]
    public static unsafe CheckedFileStream OpenFileStreamBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
    {
        return CreateFileStream(OpenBy(rootDirectory, path, options), options);
    }
}

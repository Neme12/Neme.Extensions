using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Internal.Interop;
using Neme.Extensions.InteropServices;
using Neme.Extensions.Ownership;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[UnsupportedOSPlatform("windows")]
internal sealed class UnixFileIOStrategy : FileIOStrategy
{
    protected override int MaxFileNameLength => 255;
    protected override int MaxPathLength => 4096;

    private const UnixFileMode DefaultCreateMode =
        UnixFileMode.UserRead |
        UnixFileMode.UserWrite |
        UnixFileMode.GroupRead |
        UnixFileMode.GroupWrite |
        UnixFileMode.OtherRead |
        UnixFileMode.OtherWrite;

    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandle(string path, FsFileOptions options)
    {
        ValidatePath(path);

        return Open(
            Path.GetFullPath(path),
            options.Mode,
            options.Access,
            options.Share,
            options.Options,
            DefaultCreateMode);
    }

    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandle(FsFileId fileId, FsFileOptions options)
    {
        throw new NotImplementedException();
    }

    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandleBy([Borrow] SafeFileHandle? rootDirectory, string? path, FsFileOptions options)
    {
        throw new NotImplementedException();
    }

    [return: OwnershipTransfer]
    public override SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FsFileAccess? access)
    {
        throw new NotImplementedException();
    }

    public override string GetPath([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override string GetPath(FsFileId fileId)
    {
        throw new NotImplementedException();
    }

    public override void Move([Borrow] SafeFileHandle sourceFile, string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public override void Delete([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override void SetFileAttributes([Borrow] SafeFileHandle file, FileAttributes attributes)
    {
        throw new NotImplementedException();
    }

    public override FileAttributes GetFileAttributes([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override FsFileInfo GetFileInfo([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    public override FsFileId GetFileId([Borrow] SafeFileHandle file)
    {
        throw new NotImplementedException();
    }

    private static SafeFileHandle Open(
        string fullPath,
        FileMode mode,
        FsFileAccess access,
        FileShare share,
        FileOptions options,
        UnixFileMode openPermissions)
    {
        Debug.Assert(fullPath != null);

        var openFlags =
            mode.ToUnix() |
            access.ToUnix() |
            share.ToUnix() |
            options.ToUnix();

        using var handle = OwnedOrBorrowed.Create(Interop.Libc.Open(fullPath!, openFlags, (int)openPermissions));

        if (handle.Value.IsInvalid)
        {
            handle.Dispose();

            var error = (UnixErrorCode)Marshal.GetLastPInvokeError();
            if (error == UnixErrorCode.Error_EISDIR)
                error = UnixErrorCode.Error_EACCES;

            throw UnixMarshal.GetExceptionForUnixError(new Win32Exception((int)error), fullPath);
        }

        return handle.Move();
    }
}

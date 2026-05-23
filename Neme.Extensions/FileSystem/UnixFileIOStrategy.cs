using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem;

internal sealed class UnixFileIOStrategy : FileIOStrategy
{
    [return: OwnershipTransfer]
    public override SafeFileHandle OpenHandle(string path, FsFileOptions options)
    {
        throw new NotImplementedException();
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
}

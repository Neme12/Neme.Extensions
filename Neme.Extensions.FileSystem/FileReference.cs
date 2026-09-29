using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.Ownership;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public sealed class FileReference : IFileObject, IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly string _openedPath;
    private readonly FileReferenceFlags _flags;

    private FileReference(
        [OwnershipTransfer] SafeFileHandle handle,
        string path,
        FileReferenceOptions options)
    {
        _handle = handle;
        _openedPath = path;
        _flags = options.Flags;
    }

    public string OpenedPath =>
        _openedPath;

    public FileReferenceFlags Flags =>
        _flags;

    public bool IsClosed =>
        _handle is null or { IsClosed: true };

    public string GetPath()
    {
        Require.NotDisposed(_handle is null, this);    
        return FileOperations.GetPath(_handle);
    }

    public FileId GetId()
    {
        Require.NotDisposed(_handle is null, this);
        return FileOperations.GetId(_handle);
    }

    public FileAttributes GetAttributes()
    {
        Require.NotDisposed(_handle is null, this);
        return FileOperations.GetAttributes(_handle);
    }

    public void SetAttributes(FileAttributes attributes)
    {
        Require.NotDisposed(_handle is null, this);

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.WriteAttributes, FileShare.All)))
            FileOperations.SetAttributes(handle, attributes);
    }

    public FileBasicInfo GetBasicInfo()
    {
        Require.NotDisposed(_handle is null, this);
        return FileOperations.GetBasicInfo(_handle);
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        Require.NotDisposed(_handle is null, this);

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Move(handle, destFileName, overwrite);
    }

    public void Delete()
    {
        Require.NotDisposed(_handle is null, this);

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Delete(handle);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId()
    {
        Require.NotDisposed(_handle is null, this);
        return FileOperations.GetPersistentId(_handle);
    }

    public long GetLength()
    {
        Require.NotDisposed(_handle is null, this);

        return FileOperations.GetLength(_handle);
    }

    public void SetLength(long length)
    {
        Require.NotDisposed(_handle is null, this);
        Require.ArgumentNotNegative(length);

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.Write, FileShare.All)))
            FileOperations.SetLength(handle, length);
    }

    public static FileReference Create(string path, FileReferenceOptions options = default)
    {
        var request = new FileOpenRequest(
            options.Mode.ToFileMode(),
            FileSystemAccess.ReadAttributes,
            FileShare.All,
            options.Flags.HasFlag(FileReferenceFlags.DeleteOnClose) ? FileOptions.DeleteOnClose : FileOptions.None,
            options.CreationOptions.Attributes)
        {
            PreallocationSize = options.CreationOptions.PreallocationSize,
        };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            request = request with { UnixCreateMode = options.CreationOptions.UnixCreateMode };

        var handle = FileOperations.OpenHandle(path, request);
        return new FileReference(handle, path, options);
    }

    public FileSession OpenSession(FileHandleOptions options)
    {
        Require.NotDisposed(_handle is null, this);

        var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(options));
        return new FileSession(handle, options);
    }

    public void Dispose()
    {
        if (_handle is null)
            return;

        _handle.Dispose();
        _handle = null!;
    }
}

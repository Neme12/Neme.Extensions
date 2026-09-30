using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.Ownership;
using Neme.Utilities.Contracts;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public sealed class FileReference : IFileObject, IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly string _openedPath;
    private readonly FileReferenceFlags _flags;
    private State _state;

    private FileReference(
        [OwnershipTransfer] SafeFileHandle handle,
        string path,
        FileReferenceOptions options)
    {
        _handle = handle;
        _openedPath = path;
        _flags = options.Flags;
        _state = State.Open;
    }

    public string OpenedPath
    {
        get
        {
            RequireNotDisposed();
            return _openedPath;
        }
    }

    public FileReferenceFlags Flags
    {
        get
        {
            RequireNotDisposed();
            return _flags;
        }
    }

    public bool IsClosed
    {
        get
        {
            RequireNotDisposed();
            return _state == State.Closed;
        }
    }

    public string GetPath()
    {
        RequireNotDisposed();
        RequireOpen();

        return FileOperations.GetPath(_handle);
    }

    public FileId GetId()
    {
        RequireNotDisposed();
        RequireOpen();

        return FileOperations.GetId(_handle);
    }

    public FileAttributes GetAttributes()
    {
        RequireNotDisposed();
        RequireOpen();

        return FileOperations.GetAttributes(_handle);
    }

    public void SetAttributes(FileAttributes attributes)
    {
        RequireNotDisposed();
        RequireOpen();

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.WriteAttributes, FileShare.All)))
            FileOperations.SetAttributes(handle, attributes);
    }

    public FileBasicInfo GetBasicInfo()
    {
        RequireNotDisposed();
        RequireOpen();

        return FileOperations.GetBasicInfo(_handle);
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        RequireNotDisposed();
        RequireOpen();

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Move(handle, destFileName, overwrite);
    }

    public void Delete()
    {
        RequireNotDisposed();
        RequireOpen();

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Delete(handle);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId()
    {
        RequireNotDisposed();
        RequireOpen();

        return FileOperations.GetPersistentId(_handle);
    }

    public long GetLength()
    {
        RequireNotDisposed();
        RequireOpen();

        return FileOperations.GetLength(_handle);
    }

    public void SetLength(long length)
    {
        RequireNotDisposed();
        RequireOpen();

        Require.ArgumentNotNegative(length);

        using (var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(FileSystemAccess.Write, FileShare.All)))
            FileOperations.SetLength(handle, length);
    }

    public static FileReference Create(string path, FileReferenceOptions options = default)
    {
        var request = GetFileOpenRequest(options.Mode, options.Flags) with
        {
            Attributes = options.CreationOptions.Attributes,
            PreallocationSize = options.CreationOptions.PreallocationSize,
        };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            request = request with { UnixCreateMode = options.CreationOptions.UnixCreateMode };

        var handle = FileOperations.OpenHandle(path, request);
        return new FileReference(handle, path, options);
    }

    public FileSession OpenSession(FileHandleOptions options)
    {
        RequireNotDisposed();
        RequireOpen();

        var handle = FileOperations.ReopenHandle(_handle, FileOpenRequest.Open(options));
        return new FileSession(handle, options);
    }

    public void Close()
    {
        RequireNotDisposed();
        RequireOpen();

        Debug.AssertNotNull(_handle);

        _handle.Dispose();
        _handle = null!;
        _state = State.Closed;
    }

    public void Reopen()
    {
        RequireNotDisposed();
        RequireClosed();

        Debug.AssertNull(_handle);

        var request = GetFileOpenRequest(FileReferenceMode.Open, _flags);

        _handle = FileOperations.OpenHandle(_openedPath, request);
        _state = State.Open;
    }

    public void Dispose()
    {
        if (_state == State.Disposed)
            return;

        if (_state != State.Closed)
        {
            Debug.AssertNotNull(_handle);

            _handle.Dispose();
            _handle = null!;
        }

        _state = State.Disposed;
    }

    private static FileOpenRequest GetFileOpenRequest(
        FileReferenceMode mode,
        FileReferenceFlags flags)
    {
        return new FileOpenRequest(
            mode.ToFileMode(),
            FileSystemAccess.ReadAttributes,
            FileShare.All,
            flags.HasFlag(FileReferenceFlags.DeleteOnClose)
                ? FileOptions.DeleteOnClose
                : FileOptions.None);
    }

    private void RequireNotDisposed()
    {
        Require.NotDisposed(_state == State.Disposed, this);
    }

    private void RequireOpen()
    {
        if (_state != State.Open)
            Throw.InvalidOperationException("File is closed.");
    }

    private void RequireClosed()
    {
        if (_state != State.Closed)
            Throw.InvalidOperationException("File is not closed");
    }

    private enum State : byte
    {
        Open,
        Closed,
        Disposed,
    }
}

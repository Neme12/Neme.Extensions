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
    private readonly FileReferenceOptions _options;
    private State _state;

    private FileReference(
        [OwnershipTransfer] SafeFileHandle handle,
        string path,
        FileReferenceOptions options)
    {
        Debug.Assert(handle is { IsClosed: false, IsInvalid: false });
        Debug.Assert(!handle.IsAsync);

        _handle = handle;
        _openedPath = path;
        _options = options;
        _state = State.Open;
    }

    [Owned]
    internal SafeFileHandle Handle =>
        _handle;

    public string OpenedPath
    {
        get
        {
            RequireNotDisposed();
            return _openedPath;
        }
    }

    public FileReferenceOptions Options
    {
        get
        {
            RequireNotDisposed();
            return _options;
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

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.WriteAttributes, FileShare.All)))
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

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Move(handle, destFileName, overwrite);
    }

    public void Delete()
    {
        RequireNotDisposed();
        RequireOpen();

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.Delete, FileShare.All)))
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

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.Write, FileShare.All)))
            FileOperations.SetLength(handle, length);
    }

    public static FileReference Create(string path, FileReferenceRequest request = default)
    {
        var handleRequest = GetFileOpenRequest(request.Mode, request.ReferenceOptions) with
        {
            Attributes = request.CreationOptions.Attributes,
            PreallocationSize = request.CreationOptions.PreallocationSize,
        };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            handleRequest = handleRequest with { UnixCreateMode = request.CreationOptions.UnixCreateMode };

        var handle = FileOperations.OpenHandle(path, handleRequest);
        return new FileReference(handle, path, request.ReferenceOptions);
    }

    public FileSession OpenSession(FileHandleOptions options)
    {
        RequireNotDisposed();
        RequireOpen();

        var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(options));
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

        var request = GetFileOpenRequest(FileReferenceMode.Open, _options);

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

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }

    private static FileHandleRequest GetFileOpenRequest(
        FileReferenceMode mode,
        FileReferenceOptions referenceOptions)
    {
        return new FileHandleRequest(
            mode.ToFileMode(),
            FileSystemAccess.ReadAttributes,
            FileShare.All,
            referenceOptions.Flags.HasFlag(FileReferenceFlags.DeleteOnClose)
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

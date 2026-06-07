using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public sealed class OpenFile : IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly FileOpenOptions _options;

    internal OpenFile([OwnershipTransfer] SafeFileHandle handle, FileOpenOptions options)
    {
        _handle = handle;
        _options = options;
    }

#if DEBUG
    ~OpenFile()
    {
        Debug.Fail($"{nameof(OpenFile)} should have been disposed.");
    }
#endif

    [Owned]
    public SafeFileHandle Handle
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle is null, this);
            return _handle;
        }
    }

    public FileOpenOptions Options
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle is null, this);
            return _options;
        }
    }

    public bool IsAsync
    {
        get
        {
#if NET6_0_OR_GREATER
            return _handle.IsAsync;
#else
            return (_options.Options & FileOptions.Asynchronous) != 0;
#endif
        }
    }

    public bool IsClosed =>
        _handle.IsClosed;

    public bool CanRead =>
        ((RawFileSystemAccess)_options.Access & RawFileSystemAccess.Read) != 0;

    public bool CanWrite =>
        ((RawFileSystemAccess)_options.Access & RawFileSystemAccess.Write) != 0;

    public string GetPath()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetPath(_handle);
    }

    [return: OwnershipTransfer]
    public OpenFile OpenAt(string path, FileOpenOptions options)
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.OpenAt(_handle, path, options);
    }

    public FileId GetId()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetId(_handle);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetPersistentId(_handle);
    }

    public FileAttributes GetAttributes()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetAttributes(_handle);
    }

    public void SetAttributes(FileAttributes attributes)
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        FileIO.SetAttributes(_handle, attributes);
    }

    public FileBasicInfo GetBasicInfo()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetBasicInfo(_handle);
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        FileIO.Move(_handle, destFileName, overwrite);
    }

    public void Delete()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        FileIO.Delete(_handle);
    }

    [return: OwnershipTransferUnless(nameof(leaveOpen))]
    public CheckedFileStream CreateFileStream(bool leaveOpen = false, int bufferSize = FileStreamExtensions.DefaultBufferSize)
    {
        // If the disposal will be left to the file stream, we don't need to assert disposal.
        if (!leaveOpen)
            GC.SuppressFinalize(this);

        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.CreateFileStream(_handle, _options, leaveOpen, bufferSize);
    }

    [return: OwnershipTransfer]
    public SafeFileHandle DetachHandle()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);

        GC.SuppressFinalize(this);
        var handle = _handle;
        _handle = null!;
        return handle;
    }

    public void Dispose()
    {
        if (_handle is not null)
        {
            GC.SuppressFinalize(this);
            _handle.Dispose();
            _handle = null!;
        }
    }
}

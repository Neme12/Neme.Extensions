using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Diagnostics;

namespace Neme.Extensions.FileSystem;

public sealed class FsFile : IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly FsFileOptions _options;

    public FsFile([OwnershipTransfer] SafeFileHandle handle, FsFileOptions options)
    {
        _handle = handle;
        _options = options;
    }

#if DEBUG
    ~FsFile()
    {
        Debug.Fail($"{nameof(FsFile)} should have been disposed.");
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

    public FsFileOptions Options
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle is null, this);
            return _options;
        }
    }

    [return: OwnershipTransfer]
    public CheckedFileStream CreateFileStream(bool leaveOpen = false, int bufferSize = FileStreamExtensions.DefaultBufferSize)
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.CreateFileStream(_handle, _options, leaveOpen, bufferSize);
    }

    [return: OwnershipTransfer]
    public SafeFileHandle TakeHandle()
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

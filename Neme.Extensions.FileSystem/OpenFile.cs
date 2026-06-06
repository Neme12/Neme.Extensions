using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Diagnostics;

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

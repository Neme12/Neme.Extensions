using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class LeaveOpenFileStream : CheckedFileStream
{
    private bool _disposed;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access) instead.")]
    public LeaveOpenFileStream(IntPtr handle, FileAccess access)
        : base(handle, access)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public LeaveOpenFileStream(IntPtr handle, FileAccess access, bool ownsHandle)
        : base(handle, access, ownsHandle)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public LeaveOpenFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
        : base(handle, access, ownsHandle, bufferSize)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public LeaveOpenFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
        : base(handle, access, ownsHandle, bufferSize, isAsync)
    {
    }

    public LeaveOpenFileStream(SafeFileHandle handle, FileAccess access)
        : base(handle, access)
    {
    }

    public LeaveOpenFileStream(SafeFileHandle handle, FileAccess access, int bufferSize)
        : base(handle, access, bufferSize)
    {
    }

    public LeaveOpenFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
        : base(handle, access, bufferSize, isAsync)
    {
    }

    public LeaveOpenFileStream(string path, FileMode mode)
        : base(path, mode)
    {
    }

    public LeaveOpenFileStream(string path, FileMode mode, FileAccess access)
        : base(path, mode, access)
    {
    }

    public LeaveOpenFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        : base(path, mode, access, share)
    {
    }

    public LeaveOpenFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        : base(path, mode, access, share, bufferSize)

    {
    }

    public LeaveOpenFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
        : base(path, mode, access, share, bufferSize, useAsync)
    {
    }

    public LeaveOpenFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
        : base(path, mode, access, share, bufferSize, options)
    {
    }

#if NET6_0_OR_GREATER
    public LeaveOpenFileStream(string path, FileStreamOptions options)
        : base(path, options)
    {
    }
#endif

    ~LeaveOpenFileStream()
    {
        Debug.Fail($"{nameof(LeaveOpenFileStream)} should have been disposed.");
    }

    private static bool IsIoRelatedException(Exception e) =>
        e is IOException ||
        e is UnauthorizedAccessException ||
        e is NotSupportedException ||
        e is ArgumentException && e is not ArgumentNullException;

    public override void Close()
    {
        Dispose(true);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;
#pragma warning disable CA1816
    GC.SuppressFinalize(this);
#pragma warning restore CA1816

        try
        {
            Flush();
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception e) when (!disposing && IsIoRelatedException(e))
        {
            // On finalization, ignore failures from trying to flush the write buffer,
            // e.g. if this stream is wrapping a pipe and the pipe is now broken.
        }
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        GC.SuppressFinalize(this);

        try
        {
            await FlushAsync().ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
        }
    }
#endif
}

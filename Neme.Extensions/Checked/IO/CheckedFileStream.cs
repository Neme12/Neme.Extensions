#if DEBUG
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace System.IO;

public sealed class CheckedFileStream : FileStream
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access) instead.")]
    public CheckedFileStream(IntPtr handle, FileAccess access)
        : base(handle, access)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public CheckedFileStream(IntPtr handle, FileAccess access, bool ownsHandle)
        : base(handle, access, ownsHandle)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public CheckedFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
        : base(handle, access, ownsHandle, bufferSize)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This constructor has been deprecated. Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) and optionally make a new SafeFileHandle with ownsHandle=false if needed instead.")]
    public CheckedFileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
        : base(handle, access, ownsHandle, bufferSize, isAsync)
    {
    }

    public CheckedFileStream(SafeFileHandle handle, FileAccess access)
        : base(handle, access)
    {
    }

    public CheckedFileStream(SafeFileHandle handle, FileAccess access, int bufferSize)
        : base(handle, access, bufferSize)
    {
    }

    public CheckedFileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
        : base(handle, access, bufferSize, isAsync)
    {
    }

    public CheckedFileStream(string path, FileMode mode)
        : base(path, mode)
    {
    }

    public CheckedFileStream(string path, FileMode mode, FileAccess access)
        : base(path, mode, access)
    {
    }

    public CheckedFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        : base(path, mode, access, share)
    {
    }

    public CheckedFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
        : base(path, mode, access, share, bufferSize)

    {
    }

    public CheckedFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
        : base(path, mode, access, share, bufferSize, useAsync)
    {
    }

    public CheckedFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
        : base(path, mode, access, share, bufferSize, options)
    {
    }

#if NET6_0_OR_GREATER
    public CheckedFileStream(string path, FileStreamOptions options)
        : base(path, options)
    {
    }
#endif

    ~CheckedFileStream()
    {
        Debug.Fail($"{nameof(CheckedFileStream)} should have been disposed.");
    }
}
#endif

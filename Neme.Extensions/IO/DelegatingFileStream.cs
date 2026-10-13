using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

#if NETFRAMEWORK
using System.Runtime.Remoting;
#endif

namespace Neme.Extensions.IO;

public abstract class DelegatingFileStream : CheckedFileStream
{
    private readonly FileStream _innerFileStream;

    public DelegatingFileStream([OwnershipTransfer] FileStream innerFileStream)
        : base(
            File.OpenNullHandle(),
            GetAccess(innerFileStream))
    {
        _innerFileStream = innerFileStream;
    }

    ~DelegatingFileStream()
    {
        Debug.Fail($"{nameof(DelegatingFileStream)} should have been disposed.");
    }

    protected FileStream InnerFileStream =>
        _innerFileStream;

    private static FileAccess GetAccess(FileStream fileStream)
    {
        return
            fileStream.CanRead && fileStream.CanWrite ? FileAccess.ReadWrite :
            fileStream.CanRead ? FileAccess.Read :
            fileStream.CanWrite ? FileAccess.Write :
            0;
    }

    [Obsolete("FileStream.Handle has been deprecated. Use FileStream's SafeFileHandle property instead.")]
    public override IntPtr Handle =>
        _innerFileStream.Handle;

    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("tvos")]
    [UnsupportedOSPlatform("freebsd")]
    public override void Lock(long position, long length) =>
        _innerFileStream.Lock(position, length);

    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("tvos")]
    [UnsupportedOSPlatform("freebsd")]
    public override void Unlock(long position, long length) =>
        _innerFileStream.Unlock(position, length);

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _innerFileStream.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count) =>
        _innerFileStream.Read(buffer, offset, count);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override int Read(Span<byte> buffer) =>
        _innerFileStream.Read(buffer);
#endif

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerFileStream.ReadAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        _innerFileStream.ReadAsync(buffer, cancellationToken);
#endif

    public override void Write(byte[] buffer, int offset, int count) =>
        _innerFileStream.Write(buffer, offset, count);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override void Write(ReadOnlySpan<byte> buffer) =>
        _innerFileStream.Write(buffer);
#endif

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerFileStream.WriteAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        _innerFileStream.WriteAsync(buffer, cancellationToken);
#endif

    public override void Flush() =>
        _innerFileStream.Flush();

    public override void Flush(bool flushToDisk) =>
        _innerFileStream.Flush(flushToDisk);

    public override bool CanRead =>
        // On .NET Framework, CanRead is checked in the constructor before the inner stream is assigned.
        _innerFileStream is null || _innerFileStream.CanRead;

    public override bool CanWrite =>
        // On .NET Framework, CanWrite is checked in the constructor before the inner stream is assigned.
        _innerFileStream is null || _innerFileStream.CanWrite;

    public override void SetLength(long value) =>
        _innerFileStream.SetLength(value);

    public override SafeFileHandle SafeFileHandle =>
        _innerFileStream.SafeFileHandle;

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override string Name =>
        _innerFileStream.Name;
#endif

    public override bool IsAsync =>
        _innerFileStream.IsAsync;

    public override long Length =>
        _innerFileStream.Length;

    public override long Position
    {
        get => _innerFileStream.Position;
        set => _innerFileStream.Position = value;
    }

    public override int ReadByte() =>
        _innerFileStream.ReadByte();

    public override void WriteByte(byte value) =>
        _innerFileStream.WriteByte(value);

    public override void Close()
    {
        GC.SuppressFinalize(this);
        _innerFileStream.Close();
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _innerFileStream.DisposeAsync();
    }
#endif

#if NET9_0_OR_GREATER
    public override void CopyTo(Stream destination, int bufferSize) =>
        _innerFileStream.CopyTo(destination, bufferSize);
#endif

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
        _innerFileStream.CopyToAsync(destination, bufferSize, cancellationToken);

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        _innerFileStream.BeginRead(buffer, offset, count, callback, state);

    public override int EndRead(IAsyncResult asyncResult) =>
        _innerFileStream.EndRead(asyncResult);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        _innerFileStream.BeginWrite(buffer, offset, count, callback, state);

    public override void EndWrite(IAsyncResult asyncResult) =>
        _innerFileStream.EndWrite(asyncResult);

    public override bool CanSeek =>
        _innerFileStream.CanSeek;

    public override long Seek(long offset, SeekOrigin origin) =>
        _innerFileStream.Seek(offset, origin);

    public override bool CanTimeout =>
        _innerFileStream.CanTimeout;

    public override int ReadTimeout
    {
        get => _innerFileStream.ReadTimeout;
        set => _innerFileStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => _innerFileStream.WriteTimeout;
        set => _innerFileStream.WriteTimeout = value;
    }

    protected override void Dispose(bool disposing) =>
        Accessors.Dispose(_innerFileStream, disposing);

#if NETFRAMEWORK
    public override ObjRef CreateObjRef(Type requestedType) =>
        _innerFileStream.CreateObjRef(requestedType);
#endif

#if NET5_0_OR_GREATER
    [Obsolete("This Remoting API is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0010", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override object InitializeLifetimeService() =>
        _innerFileStream.InitializeLifetimeService();

    [Obsolete("CreateWaitHandle has been deprecated. Use the ManualResetEvent(false) constructor instead.")]
    protected override WaitHandle CreateWaitHandle() =>
        Accessors.CreateWaitHandle(_innerFileStream);

    [System.Obsolete("Do not call or override this method.")]
    protected override void ObjectInvariant() =>
        Accessors.ObjectInvariant(_innerFileStream);

    private static class Accessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        public extern static void Dispose(FileStream stream, bool disposing);

        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        public extern static WaitHandle CreateWaitHandle(FileStream stream);

        [UnsafeAccessor(UnsafeAccessorKind.Method)]
        public extern static void ObjectInvariant(FileStream stream);
#else
        private const BindingFlags _bindingFlags =
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;

        public static readonly DisposeDelegate Dispose =
            typeof(FileStream).GetMethod(
                "Dispose",
                _bindingFlags,
                [typeof(bool)])!
            .CreateDelegate<DisposeDelegate>();

        public static readonly CreateWaitHandleDelegate CreateWaitHandle =
            typeof(FileStream).GetMethod(
                "CreateWaitHandle",
                _bindingFlags,
                [])!
            .CreateDelegate<CreateWaitHandleDelegate>();

        public static readonly ObjectInvariantDelegate ObjectInvariant =
            typeof(FileStream).GetMethod(
                "ObjectInvariant",
                _bindingFlags,
                [])!
            .CreateDelegate<ObjectInvariantDelegate>();

        public delegate void DisposeDelegate(FileStream stream, bool disposing);
        public delegate WaitHandle CreateWaitHandleDelegate(FileStream stream);
        public delegate void ObjectInvariantDelegate(FileStream stream);
#endif
    }
}

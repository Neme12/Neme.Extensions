using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;
using System.Diagnostics;
using System.Runtime.Versioning;

#if NETFRAMEWORK
using System.Runtime.Remoting;
#endif

namespace Neme.Extensions.IO;

public abstract class DelegatingFileStream<TStream> : CheckedFileStream
    where TStream : FileStream
{
    private readonly TStream _innerStream;

    public DelegatingFileStream([OwnershipTransfer] TStream innerStream)
        : base(
            File.OpenNullHandle(),
            GetAccess(innerStream))
    {
        _innerStream = innerStream;
    }

    ~DelegatingFileStream()
    {
        Debug.Fail($"{nameof(DelegatingFileStream<>)} should have been disposed.");
    }

    protected TStream InnerStream =>
        _innerStream;

    private static FileAccess GetAccess(TStream fileStream)
    {
        return
            fileStream.CanRead && fileStream.CanWrite ? FileAccess.ReadWrite :
            fileStream.CanRead ? FileAccess.Read :
            fileStream.CanWrite ? FileAccess.Write :
            0;
    }

    [Obsolete("FileStream.Handle has been deprecated. Use FileStream's SafeFileHandle property instead.")]
    public override IntPtr Handle =>
        _innerStream.Handle;

    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("tvos")]
    [UnsupportedOSPlatform("freebsd")]
    public override void Lock(long position, long length) =>
        _innerStream.Lock(position, length);

    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("macos")]
    [UnsupportedOSPlatform("tvos")]
    [UnsupportedOSPlatform("freebsd")]
    public override void Unlock(long position, long length) =>
        _innerStream.Unlock(position, length);

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _innerStream.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count) =>
        _innerStream.Read(buffer, offset, count);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override int Read(Span<byte> buffer) =>
        _innerStream.Read(buffer);
#endif

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        _innerStream.ReadAsync(buffer, cancellationToken);
#endif

    public override void Write(byte[] buffer, int offset, int count) =>
        _innerStream.Write(buffer, offset, count);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override void Write(ReadOnlySpan<byte> buffer) =>
        _innerStream.Write(buffer);
#endif

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        _innerStream.WriteAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        _innerStream.WriteAsync(buffer, cancellationToken);
#endif

    public override void Flush() =>
        _innerStream.Flush();

    public override void Flush(bool flushToDisk) =>
        _innerStream.Flush(flushToDisk);

    public override bool CanRead =>
        // On .NET Framework, CanRead is checked in the constructor before the inner stream is assigned.
        _innerStream is null || _innerStream.CanRead;

    public override bool CanWrite =>
        // On .NET Framework, CanWrite is checked in the constructor before the inner stream is assigned.
        _innerStream is null || _innerStream.CanWrite;

    public override void SetLength(long value) =>
        _innerStream.SetLength(value);

    public override SafeFileHandle SafeFileHandle =>
        _innerStream.SafeFileHandle;

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override string Name =>
        _innerStream.Name;
#endif

    public override bool IsAsync =>
        _innerStream.IsAsync;

    public override long Length =>
        _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public override int ReadByte() =>
        _innerStream.ReadByte();

    public override void WriteByte(byte value) =>
        _innerStream.WriteByte(value);

    public override void Close()
    {
        GC.SuppressFinalize(this);
        _innerStream.Close();
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _innerStream.DisposeAsync();
    }
#endif

#if NET9_0_OR_GREATER
    public override void CopyTo(Stream destination, int bufferSize) =>
        _innerStream.CopyTo(destination, bufferSize);
#endif

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
        _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        _innerStream.BeginRead(buffer, offset, count, callback, state);

    public override int EndRead(IAsyncResult asyncResult) =>
        _innerStream.EndRead(asyncResult);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        _innerStream.BeginWrite(buffer, offset, count, callback, state);

    public override void EndWrite(IAsyncResult asyncResult) =>
        _innerStream.EndWrite(asyncResult);

    public override bool CanSeek =>
        _innerStream.CanSeek;

    public override long Seek(long offset, SeekOrigin origin) =>
        _innerStream.Seek(offset, origin);

    public override bool CanTimeout =>
        _innerStream.CanTimeout;

    public override int ReadTimeout
    {
        get => _innerStream.ReadTimeout;
        set => _innerStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => _innerStream.WriteTimeout;
        set => _innerStream.WriteTimeout = value;
    }

    protected override void Dispose(bool disposing) =>
        StreamAccessors.Dispose(_innerStream, disposing);

#if NETFRAMEWORK
    public override ObjRef CreateObjRef(Type requestedType) =>
        _innerStream.CreateObjRef(requestedType);
#endif

#if NET5_0_OR_GREATER
    [Obsolete("This Remoting API is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0010", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    public override object InitializeLifetimeService() =>
        _innerStream.InitializeLifetimeService();

    [Obsolete("CreateWaitHandle has been deprecated. Use the ManualResetEvent(false) constructor instead.")]
    protected override WaitHandle CreateWaitHandle() =>
        StreamAccessors.CreateWaitHandle(_innerStream);

    [System.Obsolete("Do not call or override this method.")]
    protected override void ObjectInvariant() =>
        StreamAccessors.ObjectInvariant(_innerStream);
}

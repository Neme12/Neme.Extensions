using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Neme.Extensions.IO;

public class DelegatingFileStream : FileStream
{
    private readonly FileStream _innerFileStream;

    public DelegatingFileStream(FileStream innerFileStream)
        : base(innerFileStream.SafeFileHandle, GetAccess(innerFileStream))
    {
        _innerFileStream = innerFileStream;
    }

    ~DelegatingFileStream()
    {
        Debug.Fail($"{nameof(DelegatingFileStream)} should have been disposed.");
    }

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
}

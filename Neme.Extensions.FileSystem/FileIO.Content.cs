using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private static Encoding UTF8NoBOM => field ??= new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private static void ResetPosition([Borrow] Stream stream)
    {
        if (stream.CanSeek)
            stream.Seek(0, SeekOrigin.Begin);
    }

    public static string ReadAllText([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        using var _ = new StreamPositionScope(stream);
        ResetPosition(stream);
        return streamReader.ReadToEnd(cancellationToken);
    }

    public static Task<string> ReadAllTextAsync([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string>(cancellationToken);

        return CoreAsync(file, encoding, cancellationToken);

        static async Task<string> CoreAsync([Borrow] SafeFileHandle file, Encoding? encoding, CancellationToken cancellationToken)
        {
#if FILE_STREAM_ASYNC_DISPOSE
            await
#endif
            using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);

            using var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            using var _ = new StreamPositionScope(stream);
            ResetPosition(stream);
#if NET7_0_OR_GREATER
            return await streamReader.ReadToEndAsync(cancellationToken);
#else
            return await streamReader.ReadToEndAsync();
#endif
        }
    }

    public static void WriteAllText([Borrow] SafeFileHandle file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllText(file, contents.AsSpan(), encoding, cancellationToken);

    public static void WriteAllText([Borrow] SafeFileHandle file, ReadOnlySpan<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true);
        using var _ = new StreamPositionScope(stream);
        ResetPosition(stream);
        streamWriter.WriteBuffered(contents, cancellationToken);
        streamWriter.Flush();
    }

    public static Task WriteAllTextAsync([Borrow] SafeFileHandle file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllTextAsync(file, contents.AsMemory(), encoding, cancellationToken);

    public static Task WriteAllTextAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        return CoreAsync(file, contents, encoding, cancellationToken);

        static async Task CoreAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<char> contents, Encoding? encoding, CancellationToken cancellationToken = default)
        {
#if FILE_STREAM_ASYNC_DISPOSE
            await
#endif
            using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);

            using var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true);
            using var _ = new StreamPositionScope(stream);
            ResetPosition(stream);
            await streamWriter.WriteBufferedAsync(contents, cancellationToken);
#if NET8_0_OR_GREATER
            await streamWriter.FlushAsync(cancellationToken);
#else
            await streamWriter.FlushAsync();
#endif
        }
    }

    public static byte[] ReadAllBytes([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var _ = new StreamPositionScope(stream);
        ResetPosition(stream);
        return stream.ReadToEnd(cancellationToken);
    }

    public static Task<byte[]> ReadAllBytesAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, cancellationToken);

        static async Task<byte[]> CoreAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
        {
#if FILE_STREAM_ASYNC_DISPOSE
            await
#endif
            using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);

            using var _ = new StreamPositionScope(stream);
            ResetPosition(stream);
            return await stream.ReadToEndAsync(cancellationToken);
        }
    }

    public static void WriteAllBytes([Borrow] SafeFileHandle file, byte[] bytes, CancellationToken cancellationToken = default) =>
        WriteAllBytes(file, bytes.AsSpan(), cancellationToken);

    public static void WriteAllBytes([Borrow] SafeFileHandle file, ReadOnlySpan<byte> bytes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var _ = new StreamPositionScope(stream);
        ResetPosition(stream);
        stream.WriteBuffered(bytes, cancellationToken);
    }

    public static Task WriteAllBytesAsync([Borrow] SafeFileHandle file, byte[] bytes, CancellationToken cancellationToken = default) =>
        WriteAllBytesAsync(file, bytes.AsMemory(), cancellationToken);

    public static Task WriteAllBytesAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, bytes, cancellationToken);

        static async Task CoreAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        {
#if FILE_STREAM_ASYNC_DISPOSE
            await
#endif
            using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);

            using var _ = new StreamPositionScope(stream);
            ResetPosition(stream);
            await stream.WriteBufferedAsync(bytes, cancellationToken);
        }
    }

    public static void AppendAllText([Borrow] SafeFileHandle file, string? contents, Encoding? encoding, CancellationToken cancellationToken = default) =>
        AppendAllText(file, contents.AsSpan(), encoding, cancellationToken);

    public static void AppendAllText([Borrow] SafeFileHandle file, ReadOnlySpan<char> contents, Encoding? encoding, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentValid(file, !file.IsInvalid && !file.IsClosed && file.CanSeek);
        Require.ArgumentValid(file, file.CanSeek);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true);
        stream.Position = stream.Length;
        streamWriter.WriteBuffered(contents, cancellationToken);
    }

    public static Task AppendAllTextAsync([Borrow] SafeFileHandle file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        AppendAllTextAsync(file, contents.AsMemory(), encoding, cancellationToken);

    public static Task AppendAllTextAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentValid(file, !file.IsInvalid && !file.IsClosed && file.CanSeek);
        Require.ArgumentValid(file, file.CanSeek);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        return CoreAsync(file, contents, encoding, cancellationToken);

        static async Task CoreAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<char> contents, Encoding? encoding, CancellationToken cancellationToken = default)
        {
#if FILE_STREAM_ASYNC_DISPOSE
            await
#endif
            using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);

            using var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true);
            stream.Position = stream.Length;
            await streamWriter.WriteBufferedAsync(contents, cancellationToken);
        }
    }

    public static void AppendAllBytes([Borrow] SafeFileHandle file, byte[] bytes, CancellationToken cancellationToken = default) =>
        AppendAllBytes(file, bytes.AsSpan(), cancellationToken);

    public static void AppendAllBytes([Borrow] SafeFileHandle file, ReadOnlySpan<byte> bytes, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentValid(file, !file.IsInvalid && !file.IsClosed && file.CanSeek);
        Require.ArgumentValid(file, file.CanSeek);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        stream.Position = stream.Length;
        stream.WriteBuffered(bytes, cancellationToken);
    }

    public static Task AppendAllBytesAsync([Borrow] SafeFileHandle file, byte[] bytes, CancellationToken cancellationToken = default) =>
        AppendAllBytesAsync(file, bytes.AsMemory(), cancellationToken);

    public static Task AppendAllBytesAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentValid(file, !file.IsInvalid && !file.IsClosed);
        Require.ArgumentValid(file, file.CanSeek);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, bytes, cancellationToken);

        static async Task CoreAsync([Borrow] SafeFileHandle file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        {
#if FILE_STREAM_ASYNC_DISPOSE
            await
#endif
            using var stream = new LeaveOpenFileStream(file, FileAccess.Write, FileStream.DefaultBufferSize, isAsync: file.IsAsync);

            stream.Position = stream.Length;
            await stream.WriteBufferedAsync(bytes, cancellationToken);
        }
    }

    private struct StreamPositionScope : IDisposable
    {
        private Stream _stream;
        private readonly long? _initialPosition;

        public StreamPositionScope(Stream stream)
        {
            _stream = stream;
            _initialPosition = stream.CanSeek ? stream.Position : null;
        }

        public void Dispose()
        {
            if (_stream is not null)
            {
                if (_stream.CanSeek)
                    _stream.Position = _initialPosition!.Value;

                _stream = null!;
            }
        }
    }
}

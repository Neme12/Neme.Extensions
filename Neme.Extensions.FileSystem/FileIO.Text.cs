using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.InteropServices;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private static Encoding UTF8NoBOM =>
        field ??= new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public static string ReadAllText([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using (file.CreatePositionScope(0, allowNonSeekable: true))
        using (var stream = file.CreateFileStream(FileAccess.Read))
        using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
        {
            return streamReader.ReadToEnd(cancellationToken);
        }
    }

    public static Task<string> ReadAllTextAsync([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string>(cancellationToken);

        return CoreAsync(file, encoding, cancellationToken);

        static async Task<string> CoreAsync([Borrow] SafeFileHandle file, Encoding? encoding, CancellationToken cancellationToken)
        {
            using (file.CreatePositionScope(0, allowNonSeekable: true))
            await using (file.CreateFileStream(FileAccess.Read).AsAsyncDisposable(out var stream))
            using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                return await streamReader.ReadToEndAsync(cancellationToken);
            }
        }
    }

    public static void WriteAllText([Borrow] SafeFileHandle file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllText(file, contents.AsSpan(), encoding, cancellationToken);

    public static void WriteAllText([Borrow] SafeFileHandle file, ReadOnlySpan<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using (file.CreatePositionScope(0, allowNonSeekable: true))
        using (var stream = file.CreateFileStream(FileAccess.Write))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
        {
            streamWriter.WriteBuffered(contents, cancellationToken);
            streamWriter.Flush();
        }
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
            using (file.CreatePositionScope(0, allowNonSeekable: true))
            await using (file.CreateFileStream(FileAccess.Write).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
            {
                await streamWriter.WriteBufferedAsync(contents, cancellationToken);
                await streamWriter.FlushAsync(cancellationToken);
            }
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

        using (var stream = file.CreateFileStream(FileAccess.Write))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
        {
            stream.Position = stream.Length;
            streamWriter.WriteBuffered(contents, cancellationToken);
        }
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
            await using (file.CreateFileStream(FileAccess.Write).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
            {
                stream.Position = stream.Length;
                await streamWriter.WriteBufferedAsync(contents, cancellationToken);
            }
        }
    }
}

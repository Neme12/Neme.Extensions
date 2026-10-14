using Neme.Extensions.Contracts;
using Neme.Extensions.IO;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private static Encoding UTF8NoBOM =>
        field ??= new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public static string ReadAllText(FileSource file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = file.CreateFileStream(ReadOptions, resetPosition: true))
        using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
        {
            return streamReader.ReadToEnd(cancellationToken);
        }
    }

    public static Task<string> ReadAllTextAsync(FileSource file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string>(cancellationToken);

        return CoreAsync(file, encoding, cancellationToken);

        static async Task<string> CoreAsync(FileSource file, Encoding? encoding, CancellationToken cancellationToken)
        {
            await using (file.CreateFileStream(AsyncReadOptions, resetPosition: true).AsAsyncDisposable(out var stream))
            using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                return await streamReader.ReadToEndAsync(cancellationToken);
            }
        }
    }

    public static void WriteAllText(FileSource file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllText(file, contents.AsSpan(), encoding, cancellationToken);

    public static void WriteAllText(FileSource file, ReadOnlySpan<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = file.CreateFileStream(WriteOptions, resetPosition: true))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
        {
            streamWriter.WriteBuffered(contents, cancellationToken);
            streamWriter.Flush();
        }
    }

    public static Task WriteAllTextAsync(FileSource file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllTextAsync(file, contents.AsMemory(), encoding, cancellationToken);

    public static Task WriteAllTextAsync(FileSource file, ReadOnlyMemory<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        return CoreAsync(file, contents, encoding, cancellationToken);

        static async Task CoreAsync(FileSource file, ReadOnlyMemory<char> contents, Encoding? encoding, CancellationToken cancellationToken = default)
        {
            await using (file.CreateFileStream(AsyncWriteOptions, resetPosition: true).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
            {
                await streamWriter.WriteBufferedAsync(contents, cancellationToken);
                await streamWriter.FlushAsync(cancellationToken);
            }
        }
    }

    public static void AppendAllText(FileSource file, string? contents, Encoding? encoding, CancellationToken cancellationToken = default) =>
        AppendAllText(file, contents.AsSpan(), encoding, cancellationToken);

    public static void AppendAllText(FileSource file, ReadOnlySpan<char> contents, Encoding? encoding, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);
        Require.Argument(file, file.CanSeek);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = file.CreateFileStream(WriteOptions, resetPosition: false))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
        {
            stream.Position = stream.Length;
            streamWriter.WriteBuffered(contents, cancellationToken);
        }
    }

    public static Task AppendAllTextAsync(FileSource file, string? contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        AppendAllTextAsync(file, contents.AsMemory(), encoding, cancellationToken);

    public static Task AppendAllTextAsync(FileSource file, ReadOnlyMemory<char> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);
        Require.Argument(file, file.CanSeek);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        return CoreAsync(file, contents, encoding, cancellationToken);

        static async Task CoreAsync(FileSource file, ReadOnlyMemory<char> contents, Encoding? encoding, CancellationToken cancellationToken = default)
        {
            await using (file.CreateFileStream(AsyncWriteOptions, resetPosition: false).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
            {
                stream.Position = stream.Length;
                await streamWriter.WriteBufferedAsync(contents, cancellationToken);
            }
        }
    }
}

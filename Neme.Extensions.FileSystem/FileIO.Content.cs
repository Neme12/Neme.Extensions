using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.InteropServices;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private static Encoding UTF8NoBOM => field ??= new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    public static string ReadAllText([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, FileAccess.Read))
        using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
        using (stream.CreatePositionScope(0))
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
            await using (CreateFileStream(file, FileAccess.Read).AsAsyncDisposable(out var stream))
            using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            using (stream.CreatePositionScope(0))
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

        using (var stream = CreateFileStream(file, FileAccess.Write))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
        using (stream.CreatePositionScope(0))
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
            await using (CreateFileStream(file, FileAccess.Write).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
            using (stream.CreatePositionScope(0))
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

        using (var stream = CreateFileStream(file, FileAccess.Write))
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
            await using (CreateFileStream(file, FileAccess.Write).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, FileStream.DefaultBufferSize, leaveOpen: true))
            {
                stream.Position = stream.Length;
                await streamWriter.WriteBufferedAsync(contents, cancellationToken);
            }
        }
    }

    public static string[] ReadAllLines([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        var lines = new List<string>();

        using (var stream = CreateFileStream(file, FileAccess.Read))
        using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, true, StreamReader.DefaultBufferSize, leaveOpen: true))
        using (stream.CreatePositionScope(0))
        {
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                lines.Add(line);
            }
        }

        return lines.ToArray();

    }

    public static Task<string[]> ReadAllLinesAsync([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string[]>(cancellationToken);

        return CoreAsync(file, encoding, cancellationToken);

        static async Task<string[]> CoreAsync([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            var lines = new List<string>();

            await using (CreateFileStream(file, FileAccess.Read).AsAsyncDisposable(out var stream))
            using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, true, StreamReader.DefaultBufferSize, leaveOpen: true))
            using (stream.CreatePositionScope(0))
            {
                string? line;
                while ((line = await streamReader.ReadLineAsync(cancellationToken)) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }
    }

    public static void WriteAllLines([Borrow] SafeFileHandle file, string[] contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllLines(file, (IEnumerable<string>)contents, encoding, cancellationToken);

    public static void WriteAllLines([Borrow] SafeFileHandle file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentNotNull(contents);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, FileAccess.Write))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, StreamWriter.DefaultBufferSize, leaveOpen: true))
        using (stream.CreatePositionScope(0))
        {
            foreach (var line in contents)
            {
                cancellationToken.ThrowIfCancellationRequested();

                streamWriter.WriteLine(line);
            }

            streamWriter.Flush();
        }
    }

    public static Task WriteAllLinesAsync([Borrow] SafeFileHandle file, string[] contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllLinesAsync(file, (IEnumerable<string>)contents, encoding, cancellationToken);

    public static Task WriteAllLinesAsync([Borrow] SafeFileHandle file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentNotNull(contents);
        
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        return CoreAsync(file, contents, encoding, cancellationToken);

        static async Task CoreAsync([Borrow] SafeFileHandle file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            await using (CreateFileStream(file, FileAccess.Write).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, StreamWriter.DefaultBufferSize, leaveOpen: true))
            using (stream.CreatePositionScope(0))
            {
                foreach (string line in contents)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await streamWriter.WriteLineAsync(line.AsMemory(), cancellationToken);
                }

                await streamWriter.FlushAsync(cancellationToken);
            }
        }
    }

    public static byte[] ReadAllBytes([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, FileAccess.Read))
        using (stream.CreatePositionScope(0))
        {
            return stream.ReadToEnd(cancellationToken);
        }
    }

    public static Task<byte[]> ReadAllBytesAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, cancellationToken);

        static async Task<byte[]> CoreAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
        {
            await using (CreateFileStream(file, FileAccess.Read).AsAsyncDisposable(out var stream))
            using (stream.CreatePositionScope(0))
            {
                return await stream.ReadToEndAsync(cancellationToken);
            }
        }
    }

    public static void WriteAllBytes([Borrow] SafeFileHandle file, byte[] bytes, CancellationToken cancellationToken = default) =>
        WriteAllBytes(file, bytes.AsSpan(), cancellationToken);

    public static void WriteAllBytes([Borrow] SafeFileHandle file, ReadOnlySpan<byte> bytes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, FileAccess.Write))
        using (stream.CreatePositionScope(0))
        {
            stream.WriteBuffered(bytes, cancellationToken);
        }
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
            await using (CreateFileStream(file, FileAccess.Write).AsAsyncDisposable(out var stream))
            using (stream.CreatePositionScope(0))
            {
                await stream.WriteBufferedAsync(bytes, cancellationToken);
            }
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

        using (var stream = CreateFileStream(file, FileAccess.Write))
        {
            stream.Position = stream.Length;
            stream.WriteBuffered(bytes, cancellationToken);
        }
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
            await using (CreateFileStream(file, FileAccess.Write).AsAsyncDisposable(out var stream))
            {
                stream.Position = stream.Length;
                await stream.WriteBufferedAsync(bytes, cancellationToken);
            }
        }
    }
}

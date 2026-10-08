using Neme.Extensions.Contracts;
using Neme.Extensions.IO;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    public static string[] ReadAllLines(FileSource file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        cancellationToken.ThrowIfCancellationRequested();

        var lines = new List<string>();

        using (var stream = file.CreateFileStream(ReadOptions, resetPosition: true))
        using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, true, StreamReader.DefaultBufferSize, leaveOpen: true))
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

    public static Task<string[]> ReadAllLinesAsync(FileSource file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string[]>(cancellationToken);

        return CoreAsync(file, encoding, cancellationToken);

        static async Task<string[]> CoreAsync(FileSource file, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            var lines = new List<string>();

            await using (file.CreateFileStream(AsyncReadOptions, resetPosition: true).AsAsyncDisposable(out var stream))
            using (var streamReader = new StreamReader(stream, encoding ?? Encoding.UTF8, true, StreamReader.DefaultBufferSize, leaveOpen: true))
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

    public static void WriteAllLines(FileSource file, string[] contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllLines(file, (IEnumerable<string>)contents, encoding, cancellationToken);

    public static void WriteAllLines(FileSource file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);
        Require.ArgumentNotNull(contents);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = file.CreateFileStream(WriteOptions, resetPosition: true))
        using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, StreamWriter.DefaultBufferSize, leaveOpen: true))
        {
            foreach (var line in contents)
            {
                cancellationToken.ThrowIfCancellationRequested();

                streamWriter.WriteLine(line);
            }

            streamWriter.Flush();
        }
    }

    public static Task WriteAllLinesAsync(FileSource file, string[] contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllLinesAsync(file, (IEnumerable<string>)contents, encoding, cancellationToken);

    public static Task WriteAllLinesAsync(FileSource file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);
        Require.ArgumentNotNull(contents);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        return CoreAsync(file, contents, encoding, cancellationToken);

        static async Task CoreAsync(FileSource file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            await using (file.CreateFileStream(AsyncWriteOptions, resetPosition: true).AsAsyncDisposable(out var stream))
            using (var streamWriter = new StreamWriter(stream, encoding ?? UTF8NoBOM, StreamWriter.DefaultBufferSize, leaveOpen: true))
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
}

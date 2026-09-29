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
    public static string[] ReadAllLines([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        var lines = new List<string>();

        using (file.CreatePositionScope(0, allowNonSeekable: true))
        using (var stream = file.CreateFileStream(FileAccess.Read))
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

    public static Task<string[]> ReadAllLinesAsync([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string[]>(cancellationToken);

        return CoreAsync(file, encoding, cancellationToken);

        static async Task<string[]> CoreAsync([Borrow] SafeFileHandle file, Encoding? encoding = null, CancellationToken cancellationToken = default)
        {
            var lines = new List<string>();

            using (file.CreatePositionScope(0, allowNonSeekable: true))
            await using (file.CreateFileStream(FileAccess.Read).AsAsyncDisposable(out var stream))
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

    public static void WriteAllLines([Borrow] SafeFileHandle file, string[] contents, Encoding? encoding = null, CancellationToken cancellationToken = default) =>
        WriteAllLines(file, (IEnumerable<string>)contents, encoding, cancellationToken);

    public static void WriteAllLines([Borrow] SafeFileHandle file, IEnumerable<string> contents, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotNull(file);
        Require.ArgumentNotNull(contents);

        cancellationToken.ThrowIfCancellationRequested();

        using (file.CreatePositionScope(0, allowNonSeekable: true))
        using (var stream = file.CreateFileStream(FileAccess.Write))
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
            using (file.CreatePositionScope(0, allowNonSeekable: true))
            await using (file.CreateFileStream(FileAccess.Write).AsAsyncDisposable(out var stream))
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

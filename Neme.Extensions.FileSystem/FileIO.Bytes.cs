using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.InteropServices;
using Neme.Extensions.IO;
using System.Diagnostics;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private static FileHandleOptions ReadOptions =>
        new(FileSystemAccess.Read, FileShare.Read, flags: FileOptions.SequentialScan);
   
    private static FileHandleOptions AsyncReadOptions =>
        new(FileSystemAccess.Read, FileShare.Read, flags: FileOptions.SequentialScan | FileOptions.Asynchronous);

    private static FileHandleOptions WriteOptions =>
        new(FileSystemAccess.Write, FileShare.None, flags: FileOptions.SequentialScan);

    private static FileHandleOptions AsyncWriteOptions =>
        new(FileSystemAccess.Write, FileShare.None, flags: FileOptions.SequentialScan | FileOptions.Asynchronous);

    private static FileStream CreateFileStream(this FileSource file, FileHandleOptions options, bool resetPosition)
    {
        switch (file)
        {
            case FileReference reference:
                {
                    var session = reference.OpenSession(options);
                    var fileStream = session.CreateFileStream(options.Access.ToFileAccess());
                    return new FileSessionFileStream(session, fileStream);
                }
            case FileSession session:
                {
                    var fileHandle = session.Handle;
                    var fileStream = fileHandle.CreateFileStream(options.Access.ToFileAccess());
                    return resetPosition
                        ? new PositionResettingFileStream(fileStream)
                        : fileStream;
                }
            case SafeFileHandle handle:
                {
                    var fileStream = handle.CreateFileStream(options.Access.ToFileAccess());
                    return resetPosition
                        ? new PositionResettingFileStream(fileStream)
                        : fileStream;
                }
            default:
                throw new UnreachableException("Invalid FileSource type.");
        }
    }

    public static byte[] ReadAllBytes(FileSource file, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, ReadOptions, resetPosition: true))
        {
            return stream.ReadToEnd(cancellationToken);
        }
    }

    public static Task<byte[]> ReadAllBytesAsync(FileSource file, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, cancellationToken);

        static async Task<byte[]> CoreAsync(FileSource file, CancellationToken cancellationToken = default)
        {
            await using (CreateFileStream(file, AsyncReadOptions, resetPosition: true).AsAsyncDisposable(out var stream))
            {
                return await stream.ReadToEndAsync(cancellationToken);
            }
        }
    }

    public static void WriteAllBytes(FileSource file, byte[] bytes, CancellationToken cancellationToken = default) =>
        WriteAllBytes(file, bytes.AsSpan(), cancellationToken);

    public static void WriteAllBytes(FileSource file, ReadOnlySpan<byte> bytes, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, WriteOptions, resetPosition: true))
        {
            stream.WriteBuffered(bytes, cancellationToken);
        }
    }

    public static Task WriteAllBytesAsync(FileSource file, byte[] bytes, CancellationToken cancellationToken = default) =>
        WriteAllBytesAsync(file, bytes.AsMemory(), cancellationToken);

    public static Task WriteAllBytesAsync(FileSource file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, bytes, cancellationToken);

        static async Task CoreAsync(FileSource file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        {
            await using (CreateFileStream(file, AsyncWriteOptions, resetPosition: true).AsAsyncDisposable(out var stream))
            {
                await stream.WriteBufferedAsync(bytes, cancellationToken);
            }
        }
    }

    public static void AppendAllBytes(FileSource file, byte[] bytes, CancellationToken cancellationToken = default) =>
        AppendAllBytes(file, bytes.AsSpan(), cancellationToken);

    public static void AppendAllBytes(FileSource file, ReadOnlySpan<byte> bytes, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);
        Require.Argument(file, file.CanSeek);

        cancellationToken.ThrowIfCancellationRequested();

        using (var stream = CreateFileStream(file, WriteOptions, resetPosition: false))
        {
            stream.Position = stream.Length;
            stream.WriteBuffered(bytes, cancellationToken);
        }
    }

    public static Task AppendAllBytesAsync(FileSource file, byte[] bytes, CancellationToken cancellationToken = default) =>
        AppendAllBytesAsync(file, bytes.AsMemory(), cancellationToken);

    public static Task AppendAllBytesAsync(FileSource file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        Require.ArgumentNotDefault(file);
        Require.Argument(file, file.IsValid);
        Require.Argument(file, file.CanSeek);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return CoreAsync(file, bytes, cancellationToken);

        static async Task CoreAsync(FileSource file, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
        {
            await using (CreateFileStream(file, AsyncWriteOptions, resetPosition: false).AsAsyncDisposable(out var stream))
            {
                stream.Position = stream.Length;
                await stream.WriteBufferedAsync(bytes, cancellationToken);
            }
        }
    }
}

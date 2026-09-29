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
    public static byte[] ReadAllBytes([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using (file.CreatePositionScope(0, allowNonSeekable: true))
        using (var stream = file.CreateFileStream(FileAccess.Read))
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
            using (file.CreatePositionScope(0, allowNonSeekable: true))
            await using (file.CreateFileStream(FileAccess.Read).AsAsyncDisposable(out var stream))
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

        using (file.CreatePositionScope(0, allowNonSeekable: true))
        using (var stream = file.CreateFileStream(FileAccess.Write))
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
            using (file.CreatePositionScope(0, allowNonSeekable: true))
            await using (file.CreateFileStream(FileAccess.Write).AsAsyncDisposable(out var stream))
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

        using (var stream = file.CreateFileStream(FileAccess.Write))
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
            await using (file.CreateFileStream(FileAccess.Write).AsAsyncDisposable(out var stream))
            {
                stream.Position = stream.Length;
                await stream.WriteBufferedAsync(bytes, cancellationToken);
            }
        }
    }
}

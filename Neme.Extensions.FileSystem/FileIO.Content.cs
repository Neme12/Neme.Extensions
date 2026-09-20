using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    public static string ReadAllText([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default) =>
        ReadAllText(file, Encoding.UTF8, cancellationToken);

    public static string ReadAllText([Borrow] SafeFileHandle file, Encoding encoding, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(encoding);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var streamReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
        return streamReader.ReadToEnd(cancellationToken);
    }

    public static Task<string> ReadAllTextAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default) =>
        ReadAllTextAsync(file, Encoding.UTF8, cancellationToken);

    public static Task<string> ReadAllTextAsync([Borrow] SafeFileHandle file, Encoding encoding, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(encoding);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<string>(cancellationToken);

        return Core(file, encoding, cancellationToken);

        static async Task<string> Core([Borrow] SafeFileHandle file, Encoding encoding, CancellationToken cancellationToken)
        {
            using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
            using var streamReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
#if NET7_0_OR_GREATER
            return await streamReader.ReadToEndAsync(cancellationToken);
#else
            return await streamReader.ReadToEndAsync();
#endif
        }
    }

    public static byte[] ReadAllBytes([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        cancellationToken.ThrowIfCancellationRequested();

        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        return stream.ReadToEnd(cancellationToken);
    }

    public static Task<byte[]> ReadAllBytesAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        return Core(file, cancellationToken);

        static async Task<byte[]> Core([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
        {
            using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
            return await stream.ReadToEndAsync(cancellationToken);
        }
    }
}

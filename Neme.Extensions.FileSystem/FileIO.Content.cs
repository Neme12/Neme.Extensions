// Code derived from https://github.com/dotnet/runtime/blob/v11.0.0-rc.1.26425.128/src/libraries/System.Private.CoreLib/src/System/IO/File.cs

using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    public static string ReadAllText([Borrow] SafeFileHandle file) =>
        ReadAllText(file, Encoding.UTF8);

    public static string ReadAllText([Borrow] SafeFileHandle file, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(encoding);

        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using StreamReader sr = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);
        return sr.ReadToEnd();
    }

    public static Task<string> ReadAllTextAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
        => ReadAllTextAsync(file, Encoding.UTF8, cancellationToken);

    public static Task<string> ReadAllTextAsync([Borrow] SafeFileHandle file, Encoding encoding, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(encoding);

        return cancellationToken.IsCancellationRequested
            ? Task.FromCanceled<string>(cancellationToken)
            : InternalReadAllTextAsync(file, encoding, cancellationToken);
    }

    private static string InternalReadAllText([Borrow] SafeFileHandle file, Encoding encoding, CancellationToken cancellationToken)
    {
        Debug.Assert(file != null);
        Debug.Assert(encoding != null);

        char[]? buffer = null;
        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var streamReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            buffer = ArrayPool<char>.Shared.Rent(streamReader.CurrentEncoding.GetMaxCharCount(FileStream.DefaultBufferSize));
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                int read = streamReader.Read(buffer, 0, buffer.Length);
                if (read == 0)
                    return sb.ToString();

                sb.Append(buffer, 0, read);
            }
        }
        finally
        {
            if (buffer != null)
                ArrayPool<char>.Shared.Return(buffer);
        }
    }

    private static async Task<string> InternalReadAllTextAsync([Borrow] SafeFileHandle file, Encoding encoding, CancellationToken cancellationToken)
    {
        Debug.Assert(file != null);
        Debug.Assert(encoding != null);

        char[]? buffer = null;
        using var stream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
        using var streamReader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            buffer = ArrayPool<char>.Shared.Rent(streamReader.CurrentEncoding.GetMaxCharCount(FileStream.DefaultBufferSize));
            StringBuilder sb = new StringBuilder();

            while (true)
            {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                int read = await streamReader.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
#else
                int read = await streamReader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
#endif
                if (read == 0)
                    return sb.ToString();

                sb.Append(buffer, 0, read);
            }
        }
        finally
        {
            if (buffer != null)
                ArrayPool<char>.Shared.Return(buffer);
        }
    }

    public static byte[] ReadAllBytes([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        long fileLength = 0;
        if (file.CanSeek && (fileLength = file.Length) > Array.MaxLength)
        {
            throw new IOException(Strings.IO_FileTooLong2GB);
        }

#if DEBUG
        fileLength = 0; // improve the test coverage for ReadAllBytesUnknownLength
#endif

        if (fileLength == 0)
        {
            // Some file systems (e.g. procfs on Linux) return 0 for length even when there's content; also there are non-seekable files.
            // Thus we need to assume 0 doesn't mean empty.
            return ReadAllBytesUnknownLength(file, cancellationToken);
        }

#if !NET6_0_OR_GREATER
        using var fileStream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
#endif

        int index = 0;
        int count = (int)fileLength;
        byte[] bytes = new byte[count];
        while (count > 0)
        {
#if NET6_0_OR_GREATER
            int n = RandomAccess.Read(file, bytes.AsSpan(index, count), index);
#else
            int n = fileStream.Read(bytes, index, count);
#endif
            if (n == 0)
            {
                ThrowEndOfFileException();
            }

            index += n;
            count -= n;
        }
        return bytes;
    }

    private static byte[] ReadAllBytesUnknownLength(SafeFileHandle file, CancellationToken cancellationToken)
    {
#if !NET6_0_OR_GREATER
        using var fileStream = new LeaveOpenFileStream(file, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: file.IsAsync);
#endif

        byte[]? rentedArray = null;
        Span<byte> buffer = stackalloc byte[512];

        try
        {
            int bytesRead = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (bytesRead == buffer.Length)
                {
                    uint newLength = (uint)buffer.Length * 2;
                    if (newLength > Array.MaxLength)
                        newLength = (uint)Math.Max(Array.MaxLength, buffer.Length + 1);

                    byte[] tmp = ArrayPool<byte>.Shared.Rent((int)newLength);
                    buffer.CopyTo(tmp);
                    byte[]? oldRentedArray = rentedArray;
                    buffer = rentedArray = tmp;
                    if (oldRentedArray != null)
                        ArrayPool<byte>.Shared.Return(oldRentedArray);
                }

                Debug.Assert(bytesRead < buffer.Length);
#if NET6_0_OR_GREATER
                int n = RandomAccess.Read(file, buffer.Slice(bytesRead), bytesRead);
#else
                int n = fileStream.Read(buffer.Slice(bytesRead));
#endif

                if (n == 0)
                    return buffer.Slice(0, bytesRead).ToArray();

                bytesRead += n;
            }
        }
        finally
        {
            if (rentedArray != null)
                ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    public static Task<byte[]> ReadAllBytesAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<byte[]>(cancellationToken);

        long fileLength = 0L;
        if (file.CanSeek && (fileLength = file.Length) > Array.MaxLength)
            return Task.FromException<byte[]>(ExceptionDispatchInfo.SetCurrentStackTrace(new IOException(Strings.IO_FileTooLong2GB)));

#if DEBUG
        fileLength = 0; // improve the test coverage for InternalReadAllBytesUnknownLengthAsync
#endif

#pragma warning disable CA2025
        return fileLength > 0 ?
            InternalReadAllBytesAsync(file, (int)fileLength, cancellationToken) :
            InternalReadAllBytesUnknownLengthAsync(file, cancellationToken);
#pragma warning restore
    }

    private static async Task<byte[]> InternalReadAllBytesAsync(
        [Borrow] SafeFileHandle sfh,
        int count,
        CancellationToken cancellationToken)
    {
#if !NET6_0_OR_GREATER
        using var fileStream = new LeaveOpenFileStream(sfh, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: sfh.IsAsync);
#endif

        int index = 0;

        // The entire array is overwritten by the read loop below (an exception is thrown
        // if the file is truncated before all the bytes are read), so it does not need to be zeroed.
        byte[] bytes =
#if NET5_0_OR_GREATER
            GC.AllocateUninitializedArray<byte>(count);
#else
            new byte[count];
#endif

        do
        {
            int n =
#if NET6_0_OR_GREATER
                await RandomAccess.ReadAsync(sfh, bytes.AsMemory(index), index, cancellationToken).ConfigureAwait(false);
#else
                await fileStream.ReadAsync(bytes, index, bytes.Length - index, cancellationToken).ConfigureAwait(false);
#endif

            if (n == 0)
                ThrowEndOfFileException();

            index += n;
        }
        while (index < count);

        return bytes;
    }

    private static async Task<byte[]> InternalReadAllBytesUnknownLengthAsync(
        [Borrow] SafeFileHandle sfh,
        CancellationToken cancellationToken)
    {
#if !NET6_0_OR_GREATER
        using var fileStream = new LeaveOpenFileStream(sfh, FileAccess.Read, FileStream.DefaultBufferSize, isAsync: sfh.IsAsync);
#endif

        byte[] rentedArray = ArrayPool<byte>.Shared.Rent(512);

        try
        {
            int bytesRead = 0;

            while (true)
            {
                if (bytesRead == rentedArray.Length)
                {
                    uint newLength = (uint)rentedArray.Length * 2;
                    if (newLength > Array.MaxLength)
                    {
                        newLength = (uint)Math.Max(Array.MaxLength, rentedArray.Length + 1);
                    }

                    byte[] tmp = ArrayPool<byte>.Shared.Rent((int)newLength);
                    Buffer.BlockCopy(rentedArray, 0, tmp, 0, bytesRead);

                    byte[] toReturn = rentedArray;
                    rentedArray = tmp;

                    ArrayPool<byte>.Shared.Return(toReturn);
                }

                Debug.Assert(bytesRead < rentedArray.Length);

                int n =
#if NET6_0_OR_GREATER
                    await RandomAccess.ReadAsync(sfh, rentedArray.AsMemory(bytesRead), bytesRead, cancellationToken).ConfigureAwait(false);
#else
                    await fileStream.ReadAsync(rentedArray, bytesRead, rentedArray.Length - bytesRead, cancellationToken).ConfigureAwait(false);
#endif
                if (n == 0)
                    return rentedArray.AsSpan(0, bytesRead).ToArray();

                bytesRead += n;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    [DoesNotReturn]
    private static void ThrowEndOfFileException()
    {
        throw CreateEndOfFileException();
    }

    private static Exception CreateEndOfFileException() =>
        new EndOfStreamException(Strings.IO_EOF_ReadBeyondEOF);
}

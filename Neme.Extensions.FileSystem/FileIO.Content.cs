using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    public static Task<byte[]> ReadAllBytesAsync([Borrow] SafeFileHandle file, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<byte[]>(cancellationToken);
        }

        long fileLength = 0L;
        if (file.CanSeek && (fileLength = file.Length) > Array.MaxLength)
        {
            file.Dispose();
            return Task.FromException<byte[]>(ExceptionDispatchInfo.SetCurrentStackTrace(new IOException(Strings.IO_FileTooLong2GB)));
        }

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

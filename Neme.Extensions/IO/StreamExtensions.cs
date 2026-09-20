using Neme.Extensions.Contracts;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace Neme.Extensions.IO;

public static class StreamExtensions
{
    extension(Stream stream)
    {
        public byte[] ReadToEnd(CancellationToken cancellationToken = default)
        {
            Require.ArgumentNotNull(stream);

            cancellationToken.ThrowIfCancellationRequested();

            long streamLength = 0;
            if (stream.CanSeek && (streamLength = stream.Length) > Array.MaxLength)
                throw new IOException(Strings.IO_FileTooLong2GB);

#if DEBUG
            streamLength = 0; // improve the test coverage for ReadAllBytesUnknownLength
#endif

            return streamLength > 0
                ? stream.InternalReadToEnd((int)streamLength, cancellationToken)
                : stream.InternalReadToEndUnknownLength(cancellationToken);
        }

        public Task<byte[]> ReadToEndAsync(CancellationToken cancellationToken = default)
        {
            Require.ArgumentNotNull(stream);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<byte[]>(cancellationToken);

            long streamLength = 0L;
            if (stream.CanSeek && (streamLength = stream.Length) > Array.MaxLength)
                return Task.FromException<byte[]>(ExceptionDispatchInfo.SetCurrentStackTrace(new IOException(Strings.IO_FileTooLong2GB)));

#if DEBUG
            streamLength = 0; // improve the test coverage for InternalReadAllBytesUnknownLengthAsync
#endif

            return streamLength > 0 ?
                stream.InternalReadToEndAsync((int)streamLength, cancellationToken) :
                stream.InternalReadToEndUnknownLengthAsync(cancellationToken);
        }

        private byte[] InternalReadToEnd(int streamLength, CancellationToken cancellationToken)
        {
            int index = 0;
            int count = streamLength;

            byte[] bytes = new byte[count];
            while (count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int n = stream.Read(bytes, index, count);
                if (n == 0)
                {
                    ThrowEndOfFileException();
                }

                index += n;
                count -= n;
            }

            return bytes;
        }

        private byte[] InternalReadToEndUnknownLength(CancellationToken cancellationToken)
        {
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

                    int n = stream.Read(buffer.Slice(bytesRead));
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

        private async Task<byte[]> InternalReadToEndAsync(
            int streamLength,
            CancellationToken cancellationToken)
        {
            int index = 0;

            // The entire array is overwritten by the read loop below (an exception is thrown
            // if the file is truncated before all the bytes are read), so it does not need to be zeroed.
            byte[] bytes =
#if NET5_0_OR_GREATER
                GC.AllocateUninitializedArray<byte>(streamLength);
#else
                new byte[streamLength];
#endif

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                int n = await stream.ReadAsync(bytes, index, bytes.Length - index, cancellationToken).ConfigureAwait(false);
                if (n == 0)
                    ThrowEndOfFileException();

                index += n;
            }
            while (index < streamLength);

            return bytes;
        }

        private async Task<byte[]> InternalReadToEndUnknownLengthAsync(
            CancellationToken cancellationToken)
        {
            byte[] rentedArray = ArrayPool<byte>.Shared.Rent(512);

            try
            {
                int bytesRead = 0;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

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

                    int n = await stream.ReadAsync(rentedArray, bytesRead, rentedArray.Length - bytesRead, cancellationToken).ConfigureAwait(false);
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
}

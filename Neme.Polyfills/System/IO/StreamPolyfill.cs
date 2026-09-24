using System.Buffers;

namespace System.IO;

public static class StreamPolyfill
{
    extension(Stream stream)
    {
        public int Read(Span<byte> buffer)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return stream.Read(buffer);
#else
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                int numRead = stream.Read(sharedBuffer, 0, buffer.Length);
                if ((uint)numRead > (uint)buffer.Length)
                {
                    throw new IOException(SR.IO_StreamTooLong);
                }

                new ReadOnlySpan<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
                return numRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
#endif
        }

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return stream.ReadAsync(buffer, cancellationToken);
#else
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                return new ValueTask<int>(stream.ReadAsync(array.Array!, array.Offset, array.Count, cancellationToken));
            }

            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            return FinishReadAsync(stream.ReadAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer, buffer);

            static async ValueTask<int> FinishReadAsync(Task<int> readTask, byte[] localBuffer, Memory<byte> localDestination)
            {
                try
                {
                    int result = await readTask.ConfigureAwait(false);
                    new ReadOnlySpan<byte>(localBuffer, 0, result).CopyTo(localDestination.Span);
                    return result;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(localBuffer);
                }
            }
#endif
        }
#endif

#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
        public ValueTask DisposeAsync()
        {
            stream.Dispose();
            return default;
        }
#endif
    }

    extension<TStream>(TStream stream)
        where TStream : Stream
    {
        public IAsyncDisposable AsAsyncDisposable(out TStream streamOut)
        {
            streamOut = stream;

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return stream;
#else
            return new StreamAsyncDisposable(stream);
#endif
        }
    }

#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    private sealed class StreamAsyncDisposable : IAsyncDisposable
    {
        private readonly Stream _stream;

        public StreamAsyncDisposable(Stream stream)
        {
            _stream = stream;
        }

        public ValueTask DisposeAsync() =>
            _stream.DisposeAsync();
    }
#endif
}

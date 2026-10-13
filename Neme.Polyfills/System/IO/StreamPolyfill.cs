using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.IO;

public static class StreamPolyfill
{
    extension(Stream stream)
    {
        public void ReadExactly(Span<byte> buffer) =>
#if NET7_0_OR_GREATER
            stream.ReadExactly(buffer);
#else
        _ = stream.ReadAtLeastCore(buffer, buffer.Length, throwOnEndOfStream: true);
#endif

        public void ReadExactly(byte[] buffer, int offset, int count)
        {
#if NET7_0_OR_GREATER
            stream.ReadExactly(buffer, offset, count);
#else
            ValidateBufferArguments(buffer, offset, count);

            _ = stream.ReadAtLeastCore(buffer.AsSpan(offset, count), count, throwOnEndOfStream: true);
#endif
        }

        public int ReadAtLeast(Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)
        {
#if NET7_0_OR_GREATER
            return stream.ReadAtLeast(buffer, minimumBytes, throwOnEndOfStream);
#else
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return stream.ReadAtLeastCore(buffer, minimumBytes, throwOnEndOfStream);
#endif
        }

        public ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
#if NET7_0_OR_GREATER
            return stream.ReadExactlyAsync(buffer, cancellationToken);
#else
            ValueTask<int> vt = stream.ReadAtLeastAsyncCore(buffer, buffer.Length, throwOnEndOfStream: true, cancellationToken);

            // transfer the ValueTask<int> to a ValueTask without allocating here.
            return new ValueTask(vt.AsTask());
#endif
        }

        public ValueTask ReadExactlyAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
#if NET7_0_OR_GREATER
            return stream.ReadExactlyAsync(buffer, offset, count, cancellationToken);
#else
            ValidateBufferArguments(buffer, offset, count);

            ValueTask<int> vt = stream.ReadAtLeastAsyncCore(buffer.AsMemory(offset, count), count, throwOnEndOfStream: true, cancellationToken);

            // transfer the ValueTask<int> to a ValueTask without allocating here.
            return new ValueTask(vt.AsTask());
#endif
        }

        public ValueTask<int> ReadAtLeastAsync(Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default)
        {
#if NET7_0_OR_GREATER
            return stream.ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
#else
            ValidateReadAtLeastArguments(buffer.Length, minimumBytes);

            return stream.ReadAtLeastAsyncCore(buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
#endif
        }

#if !NET7_0_OR_GREATER
        private int ReadAtLeastCore(Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream)
        {
            Debug.Assert(minimumBytes <= buffer.Length);

            int totalRead = 0;
            while (totalRead < minimumBytes)
            {
                int read = stream.Read(buffer.Slice(totalRead));
                if (read == 0)
                {
                    if (throwOnEndOfStream)
                    {
                        ThrowHelper.ThrowEndOfFileException();
                    }

                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }

        private async ValueTask<int> ReadAtLeastAsyncCore(Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream, CancellationToken cancellationToken)
        {
            Debug.Assert(minimumBytes <= buffer.Length);

            int totalRead = 0;
            while (totalRead < minimumBytes)
            {
                int read = await stream.ReadAsync(buffer.Slice(totalRead), cancellationToken).ConfigureAwait(false);
                if (read == 0)
                {
                    if (throwOnEndOfStream)
                    {
                        ThrowHelper.ThrowEndOfFileException();
                    }

                    return totalRead;
                }

                totalRead += read;
            }

            return totalRead;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateBufferArguments(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.buffer);
            }

            if (offset < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.offset, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if ((uint)count > buffer.Length - offset)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.Argument_InvalidOffLen);
            }
        }

        private static void ValidateReadAtLeastArguments(int bufferLength, int minimumBytes)
        {
            if (minimumBytes < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.minimumBytes, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (bufferLength < minimumBytes)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.minimumBytes, ExceptionResource.ArgumentOutOfRange_NotGreaterThanBufferLength);
            }
        }
#endif

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
        public IAsyncDisposable AsAsyncDisposable()
        {
#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return stream;
#else
            return new StreamAsyncDisposable(stream);
#endif
        }

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

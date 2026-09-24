using System.Buffers;
using System.Runtime.InteropServices;

namespace System.IO;

public static class StreamWriterPolyfill
{
    extension(StreamWriter streamWriter)
    {
        public Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return streamWriter.WriteLineAsync(buffer, cancellationToken);
#else
            cancellationToken.ThrowIfCancellationRequested();

            if (MemoryMarshal.TryGetArray(buffer, out var arraySegment))
            {
                return streamWriter.WriteLineAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
            }
            else
            {
                var array = ArrayPool<char>.Shared.Rent(buffer.Length);

                try
                {
                    buffer.CopyTo(array);
                    return streamWriter.WriteLineAsync(array, 0, buffer.Length);
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(array);
                }
            }
#endif
        }

        public Task FlushAsync(CancellationToken cancellationToken)
        {
#if NET8_0_OR_GREATER
            return streamWriter.FlushAsync(cancellationToken);
#else
            cancellationToken.ThrowIfCancellationRequested();
            return streamWriter.FlushAsync();
#endif
        }
    }
}

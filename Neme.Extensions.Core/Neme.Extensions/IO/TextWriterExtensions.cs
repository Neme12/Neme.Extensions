using Neme.Extensions.Buffers;
using Neme.Extensions.Contracts;
using System.Buffers;

namespace Neme.Extensions.IO;

public static class TextWriterExtensions
{
    extension(TextWriter textWriter)
    {
        public void WriteBuffered(ReadOnlySpan<char> text, CancellationToken cancellationToken = default)
        {
            Require.ArgumentNotNull(textWriter);

            cancellationToken.ThrowIfCancellationRequested();

            using (var chars = ArrayPool<char>.Shared.RentLease(4096))
            {
                for (int offset = 0; offset < text.Length; offset += chars.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int length = Math.Min(chars.Length, text.Length - offset);
                    text.Slice(offset, length).CopyTo(chars.Array);

                    textWriter.Write(chars.Array, 0, length);
                }
            }
        }

        public Task WriteBufferedAsync(ReadOnlyMemory<char> text, CancellationToken cancellationToken = default)
        {
            Require.ArgumentNotNull(textWriter);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            return CoreAsync(textWriter, text, cancellationToken);

            static async Task CoreAsync(TextWriter textWriter, ReadOnlyMemory<char> text, CancellationToken cancellationToken)
            {
                using (var chars = ArrayPool<char>.Shared.RentLease(4096))
                {
                    for (int offset = 0; offset < text.Length; offset += chars.Length)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        int length = Math.Min(chars.Length, text.Length - offset);
                        text.Slice(offset, length).CopyTo(chars.Array);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                        await textWriter.WriteAsync(chars.Array.AsMemory(0, length), cancellationToken).ConfigureAwait(false);
#else
                        await textWriter.WriteAsync(chars.Array, 0, length).ConfigureAwait(false);
#endif
                    }
                }
            }
        }
    }
}

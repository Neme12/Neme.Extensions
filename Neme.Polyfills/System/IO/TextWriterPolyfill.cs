using System.Buffers;

namespace Neme.Polyfills.System.IO;

public static class TextWriterPolyfill
{
    extension(TextWriter textWriter)
    {
        public void Write(ReadOnlySpan<char> buffer)
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            textWriter.Write(buffer);
#else
            char[] array = ArrayPool<char>.Shared.Rent(buffer.Length);

            try
            {
                buffer.CopyTo(new Span<char>(array));
                textWriter.Write(array, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(array);
            }
#endif
        }
    }
}

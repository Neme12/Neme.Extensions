namespace Neme.Extensions.IO;

public static class StreamWriterExtensions
{
    private const int DefaultBufferSize = 1024;
    private const int MinBufferSize = 128;

    extension(StreamWriter)
    {
        public static int DefaultBufferSize =>
            DefaultBufferSize;

        public static int MinBufferSize =>
            MinBufferSize;
    }
}

namespace Neme.Extensions.IO;

public static class StreamReaderExtensions
{
    public const int DefaultBufferSize = 4096;
    public const int MinBufferSize = 128;

    extension(StreamReader)
    {
        public static int DefaultBufferSize =>
            DefaultBufferSize;

        public static int MinBufferSize =>
            MinBufferSize;
    }
}

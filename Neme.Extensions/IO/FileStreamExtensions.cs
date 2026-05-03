namespace Neme.Extensions.IO;

public static class FileStreamExtensions
{
    public const int DefaultBufferSize = 4096;

    extension(FileStream fileStream)
    {
        public static int DefaultBufferSize =>
            DefaultBufferSize;
    }
}

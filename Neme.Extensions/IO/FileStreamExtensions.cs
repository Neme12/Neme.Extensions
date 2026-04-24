namespace Neme.Extensions.IO;

internal static class FileStreamExtensions
{
    public const int DefaultBufferSize = 4096;

    extension(FileStream fileStream)
    {
        public static int DefaultBufferSize => 4096;
    }
}

namespace Neme.Extensions.IO;

public static class FileShareExtensions
{
    public const FileShare All = FileShare.Read | FileShare.Write | FileShare.Delete;

    extension(FileShare)
    {
        public static FileShare All => All;
    }
}

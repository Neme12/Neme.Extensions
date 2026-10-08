namespace Neme.Extensions.FileSystem;

public static class FileShareExtensions
{
    public const FileShare All = FileShare.Read | FileShare.Write | FileShare.Delete;

    extension(FileShare share)
    {
        public static FileShare All => All;
    }
}

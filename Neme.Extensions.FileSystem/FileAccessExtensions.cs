namespace Neme.Extensions.FileSystem;

public static class FileAccessExtensions
{
    public const FileAccess None = 0;

    extension(FileAccess access)
    {
        public static FileAccess None => None;
    }
}

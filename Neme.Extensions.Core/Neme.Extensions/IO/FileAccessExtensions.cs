namespace Neme.Extensions.IO;

public static class FileAccessExtensions
{
    public const FileAccess None = default;

    extension(FileAccess)
    {
        public static FileAccess None => None;
    }
}

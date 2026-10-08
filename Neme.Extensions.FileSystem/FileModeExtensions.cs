namespace Neme.Extensions.FileSystem;

public static class FileModeExtensions
{
    public const FileMode None = default;

    extension(FileMode)
    {
        public static FileMode None => None;
    }
}

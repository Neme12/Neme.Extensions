namespace Neme.Extensions.IO;

public static class FileModeExtensions
{
    public const FileMode None = default;

    extension(FileMode)
    {
        public static FileMode None => None;
    }
}

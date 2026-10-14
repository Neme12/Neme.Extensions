namespace Neme.Polyfills.System.IO;

public static class FileAttributesPolyfill
{
    public const FileAttributes None =
#if NET8_0_OR_GREATER
        FileAttributes.None;
#else
        default;
#endif

    extension(FileAttributes attributes)
    {
        public static FileAttributes None => None;
    }
}

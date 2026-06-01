namespace Neme.Extensions.FileSystem;

public static class PublicFsFileAccessExtensions
{
    extension(FsFileAccess access)
    {
        public static FsFileAccess FromFileAccess(FileAccess fileAccess)
        {
            FsFileAccess result = default;

            if ((fileAccess & FileAccess.Read) != 0)
                result |= FsFileAccess.Read;

            if ((fileAccess & FileAccess.Write) != 0)
                result |= FsFileAccess.Write;

            return result;
        }

        public FileAccess ToFileAccess()
        {
            FileAccess result = default;

            if ((access & (FsFileAccess)RawFsFileAccess.Read) != 0)
                result |= FileAccess.Read;

            if ((access & (FsFileAccess)RawFsFileAccess.Write) != 0)
                result |= FileAccess.Write;

            return result;
        }
    }
}

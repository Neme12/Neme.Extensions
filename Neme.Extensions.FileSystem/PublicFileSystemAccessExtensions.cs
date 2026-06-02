namespace Neme.Extensions.FileSystem;

public static class PublicFileSystemAccessExtensions
{
    extension(FileSystemAccess access)
    {
        public static FileSystemAccess FromFileAccess(FileAccess fileAccess)
        {
            FileSystemAccess result = default;

            if ((fileAccess & FileAccess.Read) != 0)
                result |= FileSystemAccess.Read;

            if ((fileAccess & FileAccess.Write) != 0)
                result |= FileSystemAccess.Write;

            return result;
        }

        public FileAccess ToFileAccess()
        {
            FileAccess result = default;

            if ((access & (FileSystemAccess)RawFileSystemAccess.Read) != 0)
                result |= FileAccess.Read;

            if ((access & (FileSystemAccess)RawFileSystemAccess.Write) != 0)
                result |= FileAccess.Write;

            return result;
        }
    }
}

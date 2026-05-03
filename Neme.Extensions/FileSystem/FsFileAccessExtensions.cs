using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FsFileAccessExtensions
{
    extension(FsFileAccess access)
    {
        public FILE_ACCESS_RIGHTS ToWin32()
        {
            FILE_ACCESS_RIGHTS desiredAccess = 0;

            if ((access & FsFileAccess.Read) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_READ;

            if ((access & FsFileAccess.Write) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE;

            if ((access & FsFileAccess.Delete) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.DELETE;

            if ((access & FsFileAccess.WriteAttributes) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_WRITE_ATTRIBUTES;

            return desiredAccess;

        }
    }
}

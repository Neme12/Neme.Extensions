using Mono.Unix.Native;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FsFileAccessExtensions
{
    extension(FsFileAccess access)
    {
        public FILE_ACCESS_RIGHTS ToWin32()
        {
            FILE_ACCESS_RIGHTS desiredAccess = 0;

            if ((access & FsFileAccess.ReadAttributes) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_READ_ATTRIBUTES;

            if ((access & FsFileAccess.WriteAttributes) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_WRITE_ATTRIBUTES;

            if ((access & FsFileAccess.Read) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_READ;

            if ((access & FsFileAccess.Write) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE;

            if ((access & FsFileAccess.Delete) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.DELETE;

            if ((access & FsFileAccess.Execute) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_EXECUTE;


            return desiredAccess;
        }

        public OpenFlags ToUnix()
        {
            var rawAccess = access & (FsFileAccess.Read | FsFileAccess.Write);

            return rawAccess switch
            {
                FsFileAccess.Read => OpenFlags.O_RDONLY,
                FsFileAccess.ReadWrite => OpenFlags.O_RDWR,
                FsFileAccess.Write => OpenFlags.O_WRONLY,
                _ => default,
            };
        }
    }
}

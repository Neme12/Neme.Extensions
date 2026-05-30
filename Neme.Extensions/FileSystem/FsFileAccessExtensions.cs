#if !NETFRAMEWORK
using Mono.Unix.Native;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#endif
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

#if !NETFRAMEWORK
        [UnsupportedOSPlatform("windows")]
        public OpenFlags ToUnix()
        {
            var rawAccess = (RawFsFileAccess)access;
            var readWriteAccess = rawAccess & (RawFsFileAccess.Read | RawFsFileAccess.Write);

            if (readWriteAccess != 0)
            {
                return readWriteAccess switch
                {
                    RawFsFileAccess.Read => OpenFlags.O_RDONLY,
                    RawFsFileAccess.Read | RawFsFileAccess.Write => OpenFlags.O_RDWR,
                    RawFsFileAccess.Write => OpenFlags.O_WRONLY,
                    _ => default,
                };
            }
            else
            {
                return ((rawAccess & RawFsFileAccess.WriteAttributes) != 0)
                    ? OpenFlags.O_RDONLY
                    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? OpenFlags.O_PATH
                        : default;
            }
        }
#endif
    }
}

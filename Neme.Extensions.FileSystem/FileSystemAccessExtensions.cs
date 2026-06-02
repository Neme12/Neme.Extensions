#if !NETFRAMEWORK
using Mono.Unix.Native;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#endif
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileSystemAccessExtensions
{
    extension(FileSystemAccess access)
    {
        public FILE_ACCESS_RIGHTS ToWin32()
        {
            FILE_ACCESS_RIGHTS desiredAccess = 0;

            if ((access & FileSystemAccess.ReadAttributes) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_READ_ATTRIBUTES;

            if ((access & FileSystemAccess.WriteAttributes) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_WRITE_ATTRIBUTES;

            if ((access & FileSystemAccess.Read) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_READ;

            if ((access & FileSystemAccess.Write) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE;

            if ((access & FileSystemAccess.Delete) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.DELETE;

            if ((access & FileSystemAccess.Execute) != 0)
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_EXECUTE;


            return desiredAccess;
        }

#if !NETFRAMEWORK
        [UnsupportedOSPlatform("windows")]
        public OpenFlags ToUnix()
        {
            var rawAccess = (RawFileSystemAccess)access;
            var readWriteAccess = rawAccess & (RawFileSystemAccess.Read | RawFileSystemAccess.Write);

            if (readWriteAccess != 0)
            {
                return readWriteAccess switch
                {
                    RawFileSystemAccess.Read => OpenFlags.O_RDONLY,
                    RawFileSystemAccess.Read | RawFileSystemAccess.Write => OpenFlags.O_RDWR,
                    RawFileSystemAccess.Write => OpenFlags.O_WRONLY,
                    _ => default,
                };
            }
            else
            {
                return ((rawAccess & RawFileSystemAccess.WriteAttributes) != 0)
                    ? OpenFlags.O_RDONLY
                    : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                        ? OpenFlags.O_PATH
                        : default;
            }
        }
#endif
    }
}

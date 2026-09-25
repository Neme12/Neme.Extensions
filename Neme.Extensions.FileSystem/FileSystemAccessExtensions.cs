#if !NETFRAMEWORK
using Mono.Unix.Native;
#endif
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileSystemAccessExtensions
{
    extension(FileSystemAccess access)
    {
        public FILE_ACCESS_RIGHTS ToWin32()
        {
            var rawAccess = (RawFileSystemAccess)access;

            var desiredAccess =
                FILE_ACCESS_RIGHTS.SYNCHRONIZE;

            if (rawAccess.HasFlag(RawFileSystemAccess.ReadAttributes))
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_READ_ATTRIBUTES;

            if (rawAccess.HasFlag(RawFileSystemAccess.WriteAttributes))
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_WRITE_ATTRIBUTES;

            if (rawAccess.HasFlag(RawFileSystemAccess.Read))
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_READ;

            if (rawAccess.HasFlag(RawFileSystemAccess.Write))
                desiredAccess |= FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE;

            if (rawAccess.HasFlag(RawFileSystemAccess.Delete))
                desiredAccess |= FILE_ACCESS_RIGHTS.DELETE;

            if (rawAccess.HasFlag(RawFileSystemAccess.Execute))
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

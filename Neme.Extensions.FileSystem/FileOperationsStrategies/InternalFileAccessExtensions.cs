using Windows.Win32.Storage.FileSystem;

#if !NETFRAMEWORK
using System.Runtime.Versioning;
using Mono.Unix.Native;
#endif

namespace Neme.Extensions.FileSystem.FileOperationsStrategies;

internal static class InternalFileAccessExtensions
{
    extension(FileAccess access)
    {
        public static FileAccess FromWin32(FILE_ACCESS_RIGHTS desiredAccess)
        {
            FileAccess value = FileAccess.None;

            if ((desiredAccess & FILE_ACCESS_RIGHTS.FILE_READ_DATA) != 0)
                value |= FileAccess.Read;

            if ((desiredAccess & FILE_ACCESS_RIGHTS.FILE_WRITE_DATA) != 0)
                value |= FileAccess.Write;

            return value;
        }

#if !NETFRAMEWORK
        [UnsupportedOSPlatform("windows")]
        public static FileAccess FromUnix(OpenFlags flags)
        {
            if ((flags & OpenFlags.O_PATH) != 0)
                return FileAccess.None;

            return (flags & (OpenFlags.O_RDONLY | OpenFlags.O_WRONLY | OpenFlags.O_RDWR)) switch
            {
                OpenFlags.O_RDONLY => FileAccess.Read,
                OpenFlags.O_WRONLY => FileAccess.Write,
                OpenFlags.O_RDWR => FileAccess.ReadWrite,
                _ => FileAccess.None,
            };
        }
#endif

    }
}

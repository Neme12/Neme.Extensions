#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using System.Runtime.Versioning;
using Mono.Unix;
using Mono.Unix.Native;

namespace Microsoft.Win32.SafeHandles;

public static partial class SafeFileHandlePolyfill
{
    [UnsupportedOSPlatform("windows")]
    private static class Unix
    {
        internal static FileHandleType GetFileTypeCore(SafeFileHandle handle)
        {
            int result;
            Stat status;

            bool success = false;
            handle.DangerousAddRef(ref success);

            try
            {
                result = Syscall.fstat((int)handle.DangerousGetHandle(), out status);
            }
            finally
            {
                if (success)
                    handle.DangerousRelease();
            }

            if (result != 0)
            {
                var error = Syscall.GetLastError();
                UnixMarshal.ThrowExceptionForError(error);
            }

            return MapUnixFileTypeToFileType(status);
        }

        private static FileHandleType MapUnixFileTypeToFileType(Stat status)
            => (status.st_mode & FilePermissions.S_IFMT) switch
            {
                FilePermissions.S_IFREG => FileHandleType.RegularFile,
                FilePermissions.S_IFDIR => FileHandleType.Directory,
                FilePermissions.S_IFLNK => FileHandleType.SymbolicLink,
                FilePermissions.S_IFIFO => FileHandleType.Pipe,
                FilePermissions.S_IFSOCK => FileHandleType.Socket,
                FilePermissions.S_IFCHR => FileHandleType.CharacterDevice,
                FilePermissions.S_IFBLK => FileHandleType.BlockDevice,
                _ => FileHandleType.Unknown
            };
    }
}
#endif

#if !NETFRAMEWORK
using Mono.Unix.Native;
using System.Runtime.Versioning;

#endif
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

public static class FileShareExtensions
{
    public const FileShare All = FileShare.Read | FileShare.Write | FileShare.Delete;

    extension(FileShare share)
    {
        public static FileShare All => All;

        internal FILE_SHARE_MODE ToWin32()
        {
            // The values of FileShare map directly to FILE_SHARE_MODE.
            return (FILE_SHARE_MODE)share;
        }

#if !NETFRAMEWORK
        [UnsupportedOSPlatform("windows")]
        internal OpenFlags ToUnix()
        {
            // Handle Inheritable, other FileShare flags are handled by Init
            return (share & FileShare.Inheritable) == 0 ? OpenFlags.O_CLOEXEC : default;
        }
#endif
    }
}

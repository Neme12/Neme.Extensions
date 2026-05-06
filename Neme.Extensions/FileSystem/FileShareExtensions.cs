using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileShareExtensions
{
    extension(FileShare share)
    {
        public FILE_SHARE_MODE ToWin32()
        {
            // The values of FileShare map directly to FILE_SHARE_MODE.
            return (FILE_SHARE_MODE)share;
        }
    }
}

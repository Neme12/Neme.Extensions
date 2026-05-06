using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileAttributesExtensions
{
    extension(FileAttributes attributes)
    {
        public FILE_FLAGS_AND_ATTRIBUTES ToWin32()
        {
            // The values of FileAttributes map directly to FILE_FLAGS_AND_ATTRIBUTES.
            return (FILE_FLAGS_AND_ATTRIBUTES)(uint)attributes;
        }
    }
}

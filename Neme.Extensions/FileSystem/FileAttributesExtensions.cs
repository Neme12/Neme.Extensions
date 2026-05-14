using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileAttributesExtensions
{
    extension(FileAttributes attributes)
    {
        public static FileAttributes FromWin32(FILE_FLAGS_AND_ATTRIBUTES win32Attributes)
        {
            // The values of FileAttributes map directly to FILE_FLAGS_AND_ATTRIBUTES.
            return (FileAttributes)(uint)win32Attributes;
        }

        public FILE_FLAGS_AND_ATTRIBUTES ToWin32()
        {
            // The values of FileAttributes map directly to FILE_FLAGS_AND_ATTRIBUTES.
            return (FILE_FLAGS_AND_ATTRIBUTES)(uint)attributes;
        }
    }
}

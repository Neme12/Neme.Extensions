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
            var value = (FILE_FLAGS_AND_ATTRIBUTES)(uint)attributes;

            if ((value & FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_DIRECTORY) != 0)
                value |= FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS;

            return value;
        }

        public FILE_FLAGS_AND_ATTRIBUTES ToWinNT()
        {
            // The values of FileAttributes map directly to FILE_FLAGS_AND_ATTRIBUTES.
            var value = (FILE_FLAGS_AND_ATTRIBUTES)(uint)attributes;

            // When calling NtCreateFile, we need to only specify attributes here, not flags.
            value &= ~FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS;

            return value;
        }
    }
}

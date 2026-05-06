using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileModeExtensions
{
    extension(FileMode mode)
    {
        public FILE_CREATION_DISPOSITION ToWin32()
        {
            return mode switch
            {
                FileMode.CreateNew => FILE_CREATION_DISPOSITION.CREATE_NEW,
                FileMode.Create => FILE_CREATION_DISPOSITION.CREATE_ALWAYS,
                FileMode.Open => FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                FileMode.OpenOrCreate => FILE_CREATION_DISPOSITION.OPEN_ALWAYS,
                FileMode.Truncate => FILE_CREATION_DISPOSITION.TRUNCATE_EXISTING,
                FileMode.Append => FILE_CREATION_DISPOSITION.OPEN_ALWAYS,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid FileMode value."),
            };
        }

        public NTCREATEFILE_CREATE_DISPOSITION ToWindowsNT()
        {
            return mode switch
            {
                FileMode.CreateNew => NTCREATEFILE_CREATE_DISPOSITION.FILE_CREATE,
                FileMode.Create => NTCREATEFILE_CREATE_DISPOSITION.FILE_OVERWRITE_IF,
                FileMode.Open => NTCREATEFILE_CREATE_DISPOSITION.FILE_OPEN,
                FileMode.OpenOrCreate => NTCREATEFILE_CREATE_DISPOSITION.FILE_OPEN_IF,
                FileMode.Truncate => NTCREATEFILE_CREATE_DISPOSITION.FILE_OVERWRITE,
                FileMode.Append => NTCREATEFILE_CREATE_DISPOSITION.FILE_OVERWRITE_IF,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid FileMode value."),
            };
        }

    }
}

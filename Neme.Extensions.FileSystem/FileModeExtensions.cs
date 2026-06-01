#if !NETFRAMEWORK
using Mono.Unix.Native;
using Neme.Extensions.FileSystem.FileIOStrategies;
using System.Runtime.Versioning;

#endif
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

        public NTCREATEFILE_CREATE_DISPOSITION ToWinNT()
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

#if !NETFRAMEWORK
        [UnsupportedOSPlatform("windows")]
        public OpenFlags ToUnix()
        {
            return mode switch
            {
                // if we don't lock the file, we can truncate it when opening
                // otherwise we truncate the file after getting the lock
                FileMode.CreateNew => OpenFlags.O_CREAT | OpenFlags.O_EXCL,
                FileMode.Create => OpenFlags.O_CREAT | (UnixFileIOStrategy.DisableFileLocking ? OpenFlags.O_TRUNC : 0),
                FileMode.Open => default,
                FileMode.OpenOrCreate => OpenFlags.O_CREAT,
                FileMode.Truncate => UnixFileIOStrategy.DisableFileLocking ? OpenFlags.O_TRUNC : 0,
                FileMode.Append => OpenFlags.O_CREAT,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid FileMode value."),
            };
        }
#endif
    }
}

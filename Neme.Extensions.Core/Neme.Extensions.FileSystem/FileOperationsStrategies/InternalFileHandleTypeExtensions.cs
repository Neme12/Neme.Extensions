using Windows.Win32.Storage.FileSystem;
using Windows.Wdk.Storage.FileSystem;

#if !NETFRAMEWORK
using System.Runtime.Versioning;
using Mono.Unix.Native;
#endif

namespace Neme.Extensions.FileSystem.FileOperationsStrategies;

internal static class InternalFileHandleTypeExtensions
{
    extension(FileHandleType type)
    {
        public FILE_FLAGS_AND_ATTRIBUTES ToWin32()
        {
            return type switch
            {
                FileHandleType.RegularFile =>
                    FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                FileHandleType.Directory =>
                    FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_DIRECTORY |
                    FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS,
                _ => default,
            };
        }

        public NTCREATEFILE_CREATE_OPTIONS ToWinNT()
        {
            return type switch
            {
                FileHandleType.RegularFile =>
                    NTCREATEFILE_CREATE_OPTIONS.FILE_NON_DIRECTORY_FILE,
                FileHandleType.Directory =>
                    NTCREATEFILE_CREATE_OPTIONS.FILE_DIRECTORY_FILE |
                    NTCREATEFILE_CREATE_OPTIONS.FILE_OPEN_FOR_BACKUP_INTENT,
                _ => default,
            };
        }

#if !NETFRAMEWORK
        [UnsupportedOSPlatform("windows")]
        public OpenFlags ToUnix()
        {
            return type switch
            {
                FileHandleType.RegularFile => default,
                FileHandleType.Directory => OpenFlags.O_DIRECTORY,
                _ => default,
            };
        }
#endif
    }
}

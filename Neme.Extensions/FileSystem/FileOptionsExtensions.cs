using Neme.Extensions.Internal.Interop;
using Windows.Wdk.Storage.FileSystem;
using Windows.Win32.Storage.FileSystem;

namespace Neme.Extensions.FileSystem;

internal static class FileOptionsExtensions
{
    extension(FileOptions options)
    {
        public FILE_FLAGS_AND_ATTRIBUTES ToWin32()
        {
            // The values of FileOptions map directly to FILE_FLAGS_AND_ATTRIBUTES.
            return (FILE_FLAGS_AND_ATTRIBUTES)(uint)options;
        }

        public NTCREATEFILE_CREATE_OPTIONS ToWinNT(FileAttributes attributes)
        {
            NTCREATEFILE_CREATE_OPTIONS ntOptions = 0;

            if ((options & FileOptions.WriteThrough) != 0)
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_WRITE_THROUGH;

            if ((options & FileOptions.Asynchronous) != 0)
            {
            }
            else
            {
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_SYNCHRONOUS_IO_NONALERT;
            }

            if ((options & FileOptions.RandomAccess) != 0)
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_RANDOM_ACCESS;

            if ((options & FileOptions.DeleteOnClose) != 0)
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_DELETE_ON_CLOSE;

            if ((options & FileOptions.SequentialScan) != 0)
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_SEQUENTIAL_ONLY;

            if ((options & FileOptions.Encrypted) != 0)
                throw new Exception("The Encrypted option is not supported.");

            if ((attributes & FileAttributes.Directory) != 0)
            {
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_OPEN_FOR_BACKUP_INTENT;
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_DIRECTORY_FILE;
            }
            else
            {
                ntOptions |= NTCREATEFILE_CREATE_OPTIONS.FILE_NON_DIRECTORY_FILE;
            }

            return ntOptions;
        }

        public Interop.Libc.OpenFlags ToUnix()
        {
            // Translate some FileOptions; some just aren't supported, and others will be handled after calling open.
            // - Asynchronous: Unix does not support O_NONBLOCK for regular files, only for pipes and sockets.
            // - DeleteOnClose: Doesn't have a Unix equivalent, but we approximate it in Dispose
            // - Encrypted: No equivalent on Unix and is ignored
            // - RandomAccess: Implemented after open if posix_fadvise is available
            // - SequentialScan: Implemented after open if posix_fadvise is available
            // - WriteThrough: Handled here
            return (options & FileOptions.WriteThrough) != 0 ? Interop.Libc.OpenFlags.O_SYNC : default;
        }
    }
}

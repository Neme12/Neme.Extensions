namespace Neme.Extensions.IO;

public static class FileOptionsExtensions
{
    public const FileOptions NoBuffering = (FileOptions)0x20000000; // FILE_FLAG_NO_BUFFERING
    public const FileOptions AllowPosix = (FileOptions)0x01000000;  // FILE_FLAG_POSIX_SEMANTICS
    public const FileOptions BackupOrRestore = (FileOptions)0x02000000; // FILE_FLAG_BACKUP_SEMANTICS
    public const FileOptions DisallowReparsePoint = (FileOptions)0x00200000; // FILE_FLAG_OPEN_REPARSE_POINT
    public const FileOptions NoRemoteRecall = (FileOptions)0x00100000; // FILE_FLAG_OPEN_NO_RECALL
    public const FileOptions FirstPipeInstance = (FileOptions)0x00080000; // FILE_FLAG_FIRST_PIPE_INSTANCE

    extension(FileOptions)
    {
        public static FileOptions NoBuffering => NoBuffering;
        public static FileOptions AllowPosix => AllowPosix; 
        public static FileOptions BackupOrRestore => BackupOrRestore;
        public static FileOptions DisallowReparsePoint => DisallowReparsePoint;
        public static FileOptions NoRemoteRecall => NoRemoteRecall;
        public static FileOptions FirstPipeInstance => FirstPipeInstance;
    }
}

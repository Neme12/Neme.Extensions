using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Libc
    {
        [Flags]
        internal enum FileStatusMode : uint
        {
            None = 0,

            S_IFMT = 0xF000,
            S_IFIFO = 0x1000,
            S_IFCHR = 0x2000,
            S_IFDIR = 0x4000,
            S_IFBLK = 0x6000,
            S_IFREG = 0x8000,
            S_IFLNK = 0xA000,
            S_IFSOCK = 0xC000,

            S_ISUID = 0x0800,
            S_ISGID = 0x0400,
            S_ISVTX = 0x0200,

            S_IRWXU = 0x01C0,
            S_IRUSR = 0x0100,
            S_IWUSR = 0x0080,
            S_IXUSR = 0x0040,

            S_IRWXG = 0x0038,
            S_IRGRP = 0x0020,
            S_IWGRP = 0x0010,
            S_IXGRP = 0x0008,

            S_IRWXO = 0x0007,
            S_IROTH = 0x0004,
            S_IWOTH = 0x0002,
            S_IXOTH = 0x0001,
        }

        internal static class FileStatusModeMasks
        {
            internal const uint FileTypeMask = (uint)FileStatusMode.S_IFMT;
            internal const uint FileTypeFifo = (uint)FileStatusMode.S_IFIFO;
            internal const uint FileTypeCharacterDevice = (uint)FileStatusMode.S_IFCHR;
            internal const uint FileTypeDirectory = (uint)FileStatusMode.S_IFDIR;
            internal const uint FileTypeBlockDevice = (uint)FileStatusMode.S_IFBLK;
            internal const uint FileTypeRegularFile = (uint)FileStatusMode.S_IFREG;
            internal const uint FileTypeSymbolicLink = (uint)FileStatusMode.S_IFLNK;
            internal const uint FileTypeSocket = (uint)FileStatusMode.S_IFSOCK;

            internal const uint SpecialBitsMask =
                (uint)(FileStatusMode.S_ISUID | FileStatusMode.S_ISGID | FileStatusMode.S_ISVTX);
            internal const uint SetUserId = (uint)FileStatusMode.S_ISUID;
            internal const uint SetGroupId = (uint)FileStatusMode.S_ISGID;
            internal const uint StickyBit = (uint)FileStatusMode.S_ISVTX;

            internal const uint PermissionMask =
                (uint)(FileStatusMode.S_IRWXU | FileStatusMode.S_IRWXG | FileStatusMode.S_IRWXO);
            internal const uint OwnerPermissionMask = (uint)FileStatusMode.S_IRWXU;
            internal const uint GroupPermissionMask = (uint)FileStatusMode.S_IRWXG;
            internal const uint OtherPermissionMask = (uint)FileStatusMode.S_IRWXO;

            internal const uint OwnerRead = (uint)FileStatusMode.S_IRUSR;
            internal const uint OwnerWrite = (uint)FileStatusMode.S_IWUSR;
            internal const uint OwnerExecute = (uint)FileStatusMode.S_IXUSR;

            internal const uint GroupRead = (uint)FileStatusMode.S_IRGRP;
            internal const uint GroupWrite = (uint)FileStatusMode.S_IWGRP;
            internal const uint GroupExecute = (uint)FileStatusMode.S_IXGRP;

            internal const uint OtherRead = (uint)FileStatusMode.S_IROTH;
            internal const uint OtherWrite = (uint)FileStatusMode.S_IWOTH;
            internal const uint OtherExecute = (uint)FileStatusMode.S_IXOTH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TimeSpec
        {
            internal nint tv_sec;  /* time_t */
            internal nint tv_nsec; /* long */
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FileStatus
        {
            internal ulong st_dev;     /* dev_t */
            internal nuint st_ino;     /* ino_t */
            internal FileStatusMode st_mode; /* mode_t */
            internal nuint st_nlink;   /* nlink_t */
            internal uint st_uid;      /* uid_t */
            internal uint st_gid;      /* gid_t */
            internal ulong st_rdev;    /* dev_t */
            internal nint st_size;     /* off_t */
            internal nint st_blksize;  /* blksize_t */
            internal nint st_blocks;   /* blkcnt_t */
            internal TimeSpec st_atim; /* struct timespec */
            internal TimeSpec st_mtim; /* struct timespec */
            internal TimeSpec st_ctim; /* struct timespec */
        }


        [Flags]
        internal enum FileStatusFlags
        {
            None = 0,
            HasBirthTime = 1,
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "fstat", SetLastError = true)]
        internal static partial int FStat(SafeHandle fd, out FileStatus output);
#else
        [DllImport(Libraries.libc, EntryPoint = "fstat", SetLastError = true)]
        internal static extern int FStat(SafeHandle fd, out FileStatus output);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "stat", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int Stat(string path, out FileStatus output);
#else
        [DllImport(Libraries.libc, EntryPoint = "stat", SetLastError = true)]
        internal static extern int Stat([MarshalAs(Utf8)] string path, out FileStatus output);
#endif

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "lstat", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int LStat(string path, out FileStatus output);
#else
        [DllImport(Libraries.libc, EntryPoint = "lstat", SetLastError = true)]
        internal static extern int LStat([MarshalAs(Utf8)] string path, out FileStatus output);
#endif
    }
}

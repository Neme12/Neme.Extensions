#if !NETFRAMEWORK
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Linux
    {
        [Flags]
        internal enum StatXMask : uint
        {
            Type = 0x00000001,
            Mode = 0x00000002,
            NLink = 0x00000004,
            Uid = 0x00000008,
            Gid = 0x00000010,
            ATime = 0x00000020,
            MTime = 0x00000040,
            CTime = 0x00000080,
            Ino = 0x00000100,
            Size = 0x00000200,
            Blocks = 0x00000400,
            BasicStats = 0x000007ff,
            BTime = 0x00000800,
            MntId = 0x00001000,
            DioAlign = 0x00002000,
            All = 0x00003fff,
        }

        [Flags]
        internal enum StatXFlags : int
        {
            None = 0,
            AT_SYMLINK_NOFOLLOW = 0x100,
            AT_NO_AUTOMOUNT = 0x800,
            AT_EMPTY_PATH = 0x1000,
            AT_STATX_SYNC_TYPE = 0x6000,
            AT_STATX_SYNC_AS_STAT = 0x0000,
            AT_STATX_FORCE_SYNC = 0x2000,
            AT_STATX_DONT_SYNC = 0x4000,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StatXTimestamp
        {
            internal long Seconds;
            internal uint Nanoseconds;
            internal int Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct StatXInfo
        {
            internal uint Mask;
            internal uint BlockSize;
            internal ulong Attributes;
            internal uint LinkCount;
            internal uint Uid;
            internal uint Gid;
            internal ushort Mode;
            internal ushort Reserved0;
            internal ulong Inode;
            internal ulong Size;
            internal ulong Blocks;
            internal ulong AttributesMask;
            internal StatXTimestamp AccessTime;
            internal StatXTimestamp CreationTime;
            internal StatXTimestamp ChangeTime;
            internal StatXTimestamp WriteTime;
            internal uint RDevMajor;
            internal uint RDevMinor;
            internal uint DevMajor;
            internal uint DevMinor;
            internal ulong MountId;
            internal uint DioMemAlign;
            internal uint DioOffsetAlign;
            internal fixed ulong Reserved1[12];
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_linux_shim, EntryPoint = "neme_statx", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int StatX(int dirfd, string? pathname, StatXFlags flags, StatXMask mask, out StatXInfo statx);
#else
        [DllImport(Libraries.neme_linux_shim, EntryPoint = "neme_statx", SetLastError = true)]
        internal static extern int StatX(int dirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string? pathname, StatXFlags flags, StatXMask mask, out StatXInfo statx);
#endif
    }
}
#endif

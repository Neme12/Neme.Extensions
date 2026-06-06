#if !NETFRAMEWORK
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct StatInfo
        {
            internal long Size;
            internal uint Mode;
            internal uint Uid;
            internal uint Gid;
            internal long AccessTimeSeconds;
            internal long AccessTimeNanoseconds;
            internal long WriteTimeSeconds;
            internal long WriteTimeNanoseconds;
            internal long ChangeTimeSeconds;
            internal long ChangeTimeNanoseconds;
            internal long BirthTimeSeconds;
            internal long BirthTimeNanoseconds;
            internal FileFlags Flags;
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat", SetLastError = true)]
        internal static partial int FStat(int fd, out StatInfo stat);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat", SetLastError = true)]
        internal static extern int FStat(int fd, out StatInfo stat);
#endif
    }
}
#endif

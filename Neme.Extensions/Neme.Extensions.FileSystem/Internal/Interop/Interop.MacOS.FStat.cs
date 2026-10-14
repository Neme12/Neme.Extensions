#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
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
        private static partial int FStatCore(int fd, out StatInfo stat);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat", SetLastError = true)]
        private static extern int FStatCore(int fd, out StatInfo stat);
#endif

        public static int FStat(SafeFileHandle fd, out StatInfo stat)
        {
            using (var scope = fd.CreateScope())
                return FStatCore((int)scope.Handle, out stat);
        }
    }
}
#endif

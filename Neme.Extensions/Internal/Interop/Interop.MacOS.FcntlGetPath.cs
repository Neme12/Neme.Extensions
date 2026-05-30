using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        internal const int MAXPATHLEN = 1024;
        internal const int F_GETPATH = 50;

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "fcntl", SetLastError = true)]
        internal static unsafe partial int FcntlGetPath([MarshalAs(UnmanagedType.I4)] SafeFileHandle fd, int cmd, Span<byte> path);
#else
        [DllImport(Libraries.libc, EntryPoint = "fcntl", SetLastError = true)]
        internal static unsafe extern int FcntlGetPath([MarshalAs(UnmanagedType.I4)] SafeFileHandle fd, int cmd, Span<byte> path);
#endif
    }
}

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
        internal static partial int FcntlGetPath(int fd, int cmd, Span<byte> path);
#else
        [DllImport(Libraries.libc, EntryPoint = "fcntl", SetLastError = true)]
        internal static extern int FcntlGetPath(int fd, int cmd, Span<byte> path);
#endif
    }
}

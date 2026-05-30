using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        internal const int MAXPATHLEN = 1024;

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_macos_shim, EntryPoint = "neme_fcntl_getpath", SetLastError = true)]
        internal static partial int FcntlGetPath(int fd, Span<byte> path);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fcntl_getpath", SetLastError = true)]
        internal static extern int FcntlGetPath(int fd, Span<byte> path);
#endif
    }
}

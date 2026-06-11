using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        internal const int MAXPATHLEN = 1024;

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_macos_shim, EntryPoint = "neme_fcntl_getpath", SetLastError = true)]
        private static unsafe partial int FcntlGetPathCore(int fd, byte* path);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fcntl_getpath", SetLastError = true)]
        private static unsafe extern int FcntlGetPathCore(int fd, byte* path);
#endif

        public static unsafe int FcntlGetPath(SafeFileHandle fd, byte* path)
        {
            using (var scope = fd.CreateScope())
                return FcntlGetPathCore((int)scope.Handle, path);
        }
    }
}

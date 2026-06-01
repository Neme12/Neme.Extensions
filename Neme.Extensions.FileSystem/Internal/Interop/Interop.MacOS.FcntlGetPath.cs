using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
using Neme.Extensions.InteropServices.Marshalling;
#endif

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        internal const int MAXPATHLEN = 1024;

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_macos_shim, EntryPoint = "neme_fcntl_getpath", SetLastError = true)]
        internal static unsafe partial int FcntlGetPath([MarshalUsing(typeof(SafeHandleInt32Marshaller<SafeFileHandle>))] SafeFileHandle fd, byte* path);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fcntl_getpath", SetLastError = true)]
        internal static unsafe extern int FcntlGetPath([MarshalAs(UnmanagedType.I4)] SafeFileHandle fd, byte* path);
#endif
    }
}

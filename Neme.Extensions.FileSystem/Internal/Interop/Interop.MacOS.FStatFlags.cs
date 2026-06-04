#if !NETFRAMEWORK
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat_flags", SetLastError = true)]
        internal static partial int FStatFlags(int fd, out FileFlags flags);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat_flags", SetLastError = true)]
        internal static extern int FStatFlags(int fd, out FileFlags flags);
#endif
    }
}
#endif

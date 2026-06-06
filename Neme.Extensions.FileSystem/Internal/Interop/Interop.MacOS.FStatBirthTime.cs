#if !NETFRAMEWORK
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat_birthtime", SetLastError = true)]
        internal static partial int FStatBirthTime(int fd, out long seconds, out long nanoseconds);
#else
        [DllImport(Libraries.neme_macos_shim, EntryPoint = "neme_fstat_birthtime", SetLastError = true)]
        internal static extern int FStatBirthTime(int fd, out long seconds, out long nanoseconds);
#endif
    }
}
#endif

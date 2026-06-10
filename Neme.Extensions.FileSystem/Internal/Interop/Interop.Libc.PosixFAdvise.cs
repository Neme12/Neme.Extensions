using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Libc
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "posix_fadvise", SetLastError = true)]
        internal static partial int PosixFAdvise(int fd, nint offset, nint size, int advice);
#else
        [DllImport(Libraries.libc, EntryPoint = "posix_fadvise", SetLastError = true)]
        internal static extern int PosixFAdvise(int fd, nint offset, nint size, int advice);
#endif
    }
}

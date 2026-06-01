#if !NETFRAMEWORK
using Mono.Unix.Native;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Linux
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "open_by_handle_at", SetLastError = true)]
        internal static partial int OpenByHandleAt(int mount_fd, ref FileHandleHeader handle, OpenFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "open_by_handle_at", SetLastError = true)]
        internal static extern int OpenByHandleAt(int mount_fd, ref FileHandleHeader handle, OpenFlags flags);
#endif
    }
}
#endif

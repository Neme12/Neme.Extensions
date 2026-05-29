#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Libc
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "open_by_handle_at", SetLastError = true)]
        internal static partial int OpenByHandleAt(SafeFileHandle mount_fd, ref FileHandleHeader handle, OpenFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "open_by_handle_at", SetLastError = true)]
        internal static extern int OpenByHandleAt(SafeFileHandle mount_fd, ref FileHandleHeader handle, OpenFlags flags);
#endif
    }
}
#endif

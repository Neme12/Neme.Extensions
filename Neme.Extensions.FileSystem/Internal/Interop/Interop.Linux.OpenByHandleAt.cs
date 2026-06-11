#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Linux
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "open_by_handle_at", SetLastError = true)]
        private static partial int OpenByHandleAtCore(int mount_fd, ref FileHandleHeader handle, OpenFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "open_by_handle_at", SetLastError = true)]
        private static extern int OpenByHandleAtCore(int mount_fd, ref FileHandleHeader handle, OpenFlags flags);
#endif

        public static SafeFileHandle OpenByHandleAt(SafeFileHandle mount_fd, ref FileHandleHeader handle, OpenFlags flags)
        {
            using (var mountScope = mount_fd.CreateScope())
                return new SafeFileHandle((nint)OpenByHandleAtCore((int)mountScope.Handle, ref handle, flags), ownsHandle: true);
        }
    }
}
#endif

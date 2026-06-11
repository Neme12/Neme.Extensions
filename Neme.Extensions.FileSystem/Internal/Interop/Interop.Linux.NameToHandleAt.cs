using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Linux
    {
        [Flags]
        internal enum NameToHandleAtFlags : int
        {
            None = 0,
            AT_SYMLINK_FOLLOW = 0x0400,
            AT_EMPTY_PATH = 0x1000,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FileHandleHeader
        {
            internal uint handle_bytes;
            internal int handle_type;
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "name_to_handle_at", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        private static partial int NameToHandleAtCore(int dirfd, string pathname, ref FileHandleHeader handle, out int mount_id, NameToHandleAtFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "name_to_handle_at", SetLastError = true)]
        private static extern int NameToHandleAtCore(int dirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string pathname, ref FileHandleHeader handle, out int mount_id, NameToHandleAtFlags flags);
#endif

        public static int NameToHandleAt(SafeFileHandle dirfd, string pathname, ref FileHandleHeader handle, out int mount_id, NameToHandleAtFlags flags)
        {
            using (var dirScope = dirfd.CreateScope())
                return NameToHandleAtCore((int)dirScope.Handle, pathname, ref handle, out mount_id, flags);
        }
    }
}

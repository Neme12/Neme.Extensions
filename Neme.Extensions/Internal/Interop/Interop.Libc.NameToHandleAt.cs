using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Libc
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

        internal const int AT_FDCWD = -100;

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "name_to_handle_at", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static unsafe partial int NameToHandleAt(int dirfd, string pathname, FileHandleHeader* handle, out int mount_id, NameToHandleAtFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "name_to_handle_at", SetLastError = true)]
        internal static unsafe extern int NameToHandleAt(int dirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string pathname, FileHandleHeader* handle, out int mount_id, NameToHandleAtFlags flags);
#endif
    }
}

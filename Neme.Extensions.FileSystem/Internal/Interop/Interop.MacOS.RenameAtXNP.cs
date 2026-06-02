#if !NETFRAMEWORK
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        [Flags]
        internal enum RenameAtXNPFlags : uint
        {
            None = 0,
            RENAME_SECLUDE = 0x00000001,
            RENAME_SWAP = 0x00000002,
            RENAME_EXCL = 0x00000004,
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "renameatx_np", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int RenameAtXNp(int fromFd, string from, int toFd, string to, RenameAtXNPFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "renameatx_np", SetLastError = true)]
        internal static extern int RenameAtXNp(int fromFd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string from, int toFd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string to, RenameAtXNPFlags flags);
#endif
    }
}
#endif

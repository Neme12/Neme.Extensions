#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class MacOS
    {
        [Flags]
        internal enum RenameAtXNpFlags : uint
        {
            None = 0,
            RENAME_SECLUDE = 0x00000001,
            RENAME_SWAP = 0x00000002,
            RENAME_EXCL = 0x00000004,
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "renameatx_np", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        private static partial int RenameAtXNpCore(int fromFd, string from, int toFd, string to, RenameAtXNpFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "renameatx_np", SetLastError = true)]
        private static extern int RenameAtXNpCore(int fromFd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string from, int toFd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string to, RenameAtXNpFlags flags);
#endif

        public static int RenameAtXNp(SafeFileHandle fromFd, string from, SafeFileHandle toFd, string to, RenameAtXNpFlags flags)
        {
            using (var fromScope = fromFd.CreateScope())
            using (var toScope = toFd.CreateScope())
            {
                return RenameAtXNpCore((int)fromScope.Handle, from, (int)toScope.Handle, to, flags);
            }
        }
    }
}
#endif

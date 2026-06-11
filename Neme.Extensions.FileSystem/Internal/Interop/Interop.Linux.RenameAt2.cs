#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Linux
    {
        [Flags]
        internal enum RenameAt2Flags : uint
        {
            None = 0,
            RENAME_NOREPLACE = 1u << 0,
            RENAME_EXCHANGE = 1u << 1,
            RENAME_WHITEOUT = 1u << 2,
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "renameat2", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        private static partial int RenameAt2Core(int olddirfd, string oldpath, int newdirfd, string newpath, RenameAt2Flags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "renameat2", SetLastError = true)]
        private static extern int RenameAt2Core(int olddirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string oldpath, int newdirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string newpath, RenameAt2Flags flags);
#endif

        public static int RenameAt2(SafeFileHandle olddirfd, string oldpath, SafeFileHandle newdirfd, string newpath, RenameAt2Flags flags)
        {
            using (var olddirScope = olddirfd.CreateScope())
            using (var newdirScope = newdirfd.CreateScope())
                return RenameAt2Core((int)olddirScope.Handle, oldpath, (int)newdirScope.Handle, newpath, flags);
        }
    }
}
#endif

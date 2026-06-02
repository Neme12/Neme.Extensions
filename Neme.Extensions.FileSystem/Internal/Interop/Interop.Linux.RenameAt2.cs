#if !NETFRAMEWORK
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
        internal static partial int RenameAt2(int olddirfd, string oldpath, int newdirfd, string newpath, RenameAt2Flags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "renameat2", SetLastError = true)]
        internal static extern int RenameAt2(int olddirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string oldpath, int newdirfd, [MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string newpath, RenameAt2Flags flags);
#endif
    }
}
#endif

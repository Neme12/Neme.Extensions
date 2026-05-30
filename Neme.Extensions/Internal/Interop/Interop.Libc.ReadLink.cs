using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Libc
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "readlink", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static unsafe partial nint ReadLink(string path, byte* buffer, nuint bufferSize);
#else
        [DllImport(Libraries.libc, EntryPoint = "readlink", SetLastError = true)]
        internal static unsafe extern nint ReadLink([MarshalAs(UnmanagedTypePolyfill.LPUTF8Str)] string path, byte* buffer, nuint bufferSize);
#endif
    }
}

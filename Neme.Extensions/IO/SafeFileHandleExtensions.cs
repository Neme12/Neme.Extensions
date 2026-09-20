using Microsoft.Win32.SafeHandles;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.IO;

public static class SafeFileHandleExtensions
{
    extension(SafeFileHandle handle)
    {
        public bool CanSeek =>
#if NET8_0_OR_GREATER
            SafeFileHandleAccessors.CanSeek(handle);
#else
            new FileStream(handle, FileAccess.Read).CanSeek;
#endif

        public long Length =>
#if NET6_0_OR_GREATER
            RandomAccess.GetLength(handle);
#else
            new FileStream(handle, FileAccess.Read).Length;
#endif

    }

    private static class SafeFileHandleAccessors
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_CanSeek")]
        public extern static bool CanSeek(SafeFileHandle handle);
#endif
    }
}

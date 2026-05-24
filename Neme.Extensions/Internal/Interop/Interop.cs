using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    private const UnmanagedType Utf8 =
#if NETSTANDARD2_0
        (UnmanagedType)48;
#else
        UnmanagedType.LPUTF8Str;
#endif
}

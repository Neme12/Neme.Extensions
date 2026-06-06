namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static class Libraries
    {
        internal const string libc = "libc";
        internal const string neme_linux_shim = "neme_linux_shim";
        internal const string neme_macos_shim = "neme_macos_shim";
    }

    internal static partial class Libc
    {
        internal const int AT_FDCWD = -100;
    }
}

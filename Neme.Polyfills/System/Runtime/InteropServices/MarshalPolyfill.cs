using System.Runtime.InteropServices;

namespace Neme.Polyfills.System.Runtime.InteropServices;

#if !NET6_0_OR_GREATER

public static class MarshalPolyfill
{
    extension(Marshal)
    {
        public static int GetLastPInvokeError() =>
            Marshal.GetLastWin32Error();
    }
}

#endif

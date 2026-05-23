using System.Runtime.InteropServices;
using System.Text;

namespace Neme.Polyfills.System.Runtime.InteropServices;


public static class MarshalPolyfill
{
    extension(Marshal)
    {
#if !NET6_0_OR_GREATER
        public static int GetLastPInvokeError() =>
            Marshal.GetLastWin32Error();
#endif

#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
        // Win32 has the concept of Atoms, where a pointer can either be a pointer
        // or an int.  If it's less than 64K, this is guaranteed to NOT be a
        // pointer since the bottom 64K bytes are reserved in a process' page table.
        // We should be careful about deallocating this stuff.
        private static bool IsNullOrWin32Atom(IntPtr ptr)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const long HIWORDMASK = unchecked((long)0xffffffffffff0000L);

                long lPtr = (long)ptr;
                return 0 == (lPtr & HIWORDMASK);
            }
            else
            {
                return ptr == IntPtr.Zero;
            }
        }

        public static unsafe string? PtrToStringUTF8(IntPtr ptr)
        {
            if (IsNullOrWin32Atom(ptr))
            {
                return null;
            }

            int nbBytes = string.strlen((byte*)ptr);
            return Encoding.UTF8.GetString((byte*)ptr, nbBytes);
        }
#endif
    }
}


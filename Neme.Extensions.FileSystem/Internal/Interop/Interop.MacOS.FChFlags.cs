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
        internal enum FileFlags : uint
        {
            None = 0,
            UF_SETTABLE = 0x0000ffff,
            UF_NODUMP = 0x00000001,
            UF_IMMUTABLE = 0x00000002,
            UF_APPEND = 0x00000004,
            UF_OPAQUE = 0x00000008,
            UF_COMPRESSED = 0x00000020,
            UF_TRACKED = 0x00000040,
            UF_DATAVAULT = 0x00000080,
            UF_HIDDEN = 0x00008000,
            SF_SUPPORTED = 0x009f0000,
            SF_SETTABLE = 0x3fff0000,
            SF_SYNTHETIC = 0xc0000000,
            SF_ARCHIVED = 0x00010000,
            SF_IMMUTABLE = 0x00020000,
            SF_APPEND = 0x00040000,
            SF_RESTRICTED = 0x00080000,
            SF_NOUNLINK = 0x00100000,
            SF_FIRMLINK = 0x00800000,
            SF_DATALESS = 0x40000000,
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "fchflags", SetLastError = true)]
        private static partial int FChFlagsCore(int fd, FileFlags flags);
#else
        [DllImport(Libraries.libc, EntryPoint = "fchflags", SetLastError = true)]
        private static extern int FChFlagsCore(int fd, FileFlags flags);
#endif

        public static int FChFlags(SafeFileHandle fd, FileFlags flags)
        {
            using (var scope = fd.CreateScope())
                return FChFlagsCore((int)scope.Handle, flags);
        }
    }
}
#endif

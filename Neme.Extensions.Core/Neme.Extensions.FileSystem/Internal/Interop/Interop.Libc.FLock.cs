using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal.Interop;

internal static partial class Interop
{
    internal static partial class Libc
    {
        [Flags]
        internal enum LockOperations : int
        {
            LOCK_SH = 1,    /* shared lock */
            LOCK_EX = 2,    /* exclusive lock */
            LOCK_NB = 4,    /* don't block when locking*/
            LOCK_UN = 8,    /* unlock */
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.libc, EntryPoint = "flock", SetLastError = true)]
        private static partial int FLockCore(int fd, LockOperations operation);
#else
        [DllImport(Libraries.libc, EntryPoint = "flock", SetLastError = true)]
        private static extern int FLockCore(int fd, LockOperations operation);
#endif

        public static int FLock(SafeFileHandle fd, LockOperations operation)
        {
            using (var scope = fd.CreateScope())
                return FLockCore((int)scope.Handle, operation);
        }
    }
}

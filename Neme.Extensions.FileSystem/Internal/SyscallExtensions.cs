#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.Internal;

internal static class SyscallExtensions
{
    extension(Syscall)
    {
        public static SafeFileHandle dup(SafeFileHandle fd)
        {
            using (var scope = fd.CreateScope())
                return new SafeFileHandle((nint)Syscall.dup((int)scope.Handle), ownsHandle: true);
        }

        public static SafeFileHandle open_handle(
            string pathname, OpenFlags flags, FilePermissions mode)
        {
            return new SafeFileHandle((nint)Syscall.open(pathname, flags, mode), ownsHandle: true);
        }

        public static SafeFileHandle openat(SafeFileHandle dirfd, string pathname, OpenFlags flags, FilePermissions mode)
        {
            using (var dirScope = dirfd.CreateScope())
                return new SafeFileHandle((nint)Syscall.openat((int)dirScope.Handle, pathname, flags, mode), ownsHandle: true);
        }

        public static int fstat(SafeFileHandle filedes, out Stat buf)
        {
            using (var scope = filedes.CreateScope())
                return Syscall.fstat((int)scope.Handle, out buf);
        }

        public static int fstatat(SafeFileHandle dirfd, string file_name, out Stat buf, AtFlags flags)
        {
            using (var dirScope = dirfd.CreateScope())
                return Syscall.fstatat((int)dirScope.Handle, file_name, out buf, flags);
        }

        public static int posix_fadvise(SafeFileHandle fd, long offset,
            long len, PosixFadviseAdvice advice)
        {
            using (var scope = fd.CreateScope())
                return Syscall.posix_fadvise((int)scope.Handle, offset, len, advice);
        }

        public static int posix_fallocate(SafeFileHandle fd, long offset, ulong len)
        {
            using (var scope = fd.CreateScope())
                return Syscall.posix_fallocate((int)scope.Handle, offset, len);
        }

        public static int ftruncate(SafeFileHandle fd, long length)
        {
            using (var scope = fd.CreateScope())
                return Syscall.ftruncate((int)scope.Handle, length);
        }

        public static int fchmod(SafeFileHandle filedes, FilePermissions mode)
        {
            using (var scope = filedes.CreateScope())
                return Syscall.fchmod((int)scope.Handle, mode);
        }

        public static int unlinkat(SafeFileHandle dirfd, string pathname, AtFlags flags)
        {
            using (var dirScope = dirfd.CreateScope())
                return Syscall.unlinkat((int)dirScope.Handle, pathname, flags);
        }
    }
}
#endif

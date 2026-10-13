#if !NETFRAMEWORK
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;
using System.Runtime.Versioning;

namespace System.IO;

public static partial class FilePolyfill
{
    [UnsupportedOSPlatform("windows")]
    internal static class Unix
    {
        public const string NullDevicePath = "/dev/null";

        private const UnixFileMode DefaultCreateMode =
            UnixFileMode.UserRead |
            UnixFileMode.UserWrite |
            UnixFileMode.GroupRead |
            UnixFileMode.GroupWrite |
            UnixFileMode.OtherRead |
            UnixFileMode.OtherWrite;

        public static SafeFileHandle OpenNullHandle()
        {
            using var handle = new SafeFileHandle(
                (nint)Syscall.open(NullDevicePath, OpenFlags.O_RDWR, (FilePermissions)DefaultCreateMode),
                ownsHandle: true);

#if NET8_0_OR_GREATER
            SafeFileHandleAccessors.Path(handle) = NullDevicePath;
#else
            SafeFileHandleAccessors.PathField?.SetValue(handle, NullDevicePath);
#endif

            return handle;
        }
    }
}
#endif

using Microsoft.Win32.SafeHandles;
using Neme.Extensions.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;

namespace System.IO;

public static partial class FilePolyfill
{
    [SupportedOSPlatform("windows5.1.2600")]
    internal static class Windows
    {
        public const string NullDevicePath = "NUL";

        public static SafeFileHandle OpenNullHandle()
        {
            var handle = PInvoke.CreateFile(
                NullDevicePath,
                (uint)(FILE_ACCESS_RIGHTS.FILE_GENERIC_READ | FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE),
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                lpSecurityAttributes: null,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                dwFlagsAndAttributes: default,
                hTemplateFile: null);

            if (handle.IsInvalid)
                throw Win32Marshal.GetExceptionForLastWin32Error(NullDevicePath);

#if NET8_0_OR_GREATER
            SafeFileHandleAccessors.Path(handle) = NullDevicePath;
#else
            SafeFileHandleAccessors.PathField?.SetValue(handle, NullDevicePath);
#endif

            return handle;
        }
    }
}

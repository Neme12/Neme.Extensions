#if !NETFRAMEWORK
using Mono.Unix.Native;
using Neme.Extensions.InteropServices;
using System.ComponentModel;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Internal;

[UnsupportedOSPlatform("windows")]
internal static class UnixMarshalExtensions
{
    extension(UnixMarshal)
    {
        public static Exception GetExceptionForUnixError(Errno errorCode, string? path = "", bool isDirError = false) =>
            UnixMarshal.GetExceptionForUnixError((int)errorCode, path, isDirError);

        public static Exception GetExceptionForLastStdlibError(string? path = "", bool isDirError = false) =>
            GetExceptionForUnixError(Stdlib.GetLastError(), path, isDirError);
    }
}
#endif

using System.ComponentModel;
using System.Runtime.Versioning;
using Windows.Win32.Foundation;

using Win32PInvoke = Windows.Win32.PInvoke;

namespace Neme.Extensions.InteropServices;

[SupportedOSPlatform("windows")]
public static class WinNtMarshal
{
    [SupportedOSPlatform("windows5.1.2600")]
    public static Exception GetExceptionForNtStatus(int status, string? path = "")
    {
        var win32Error = Win32PInvoke.RtlNtStatusToDosError((NTSTATUS)status);
        var win32Exception = new Win32Exception(unchecked((int)win32Error));
        throw Win32Marshal.GetExceptionForWin32Error(win32Exception, path);
    }
}

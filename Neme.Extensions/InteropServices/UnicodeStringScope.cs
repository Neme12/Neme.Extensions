using System.Runtime.Versioning;
using Windows.Win32.Foundation;
using Win32PInvoke = Windows.Win32.PInvoke;

namespace Neme.Extensions.InteropServices;

[SupportedOSPlatform("windows5.0")]
internal unsafe struct UnicodeStringScope : IDisposable
{
    private UNICODE_STRING* _unicodeString;

    public UnicodeStringScope(UNICODE_STRING* unicodeString, string str)
    {
        Win32PInvoke.RtlInitUnicodeString(ref *unicodeString, str);
        _unicodeString = unicodeString;
    }

    public void Dispose()
    {
        if (_unicodeString is not null)
        {
            Win32PInvoke.RtlFreeUnicodeString(ref *_unicodeString);
            _unicodeString = null;
        }
    }
}

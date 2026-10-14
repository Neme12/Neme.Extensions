using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.SafeHandles;

public static class SafeFileHandleExtensions
{
    extension(SafeFileHandle handle)
    {
        public bool IsOpen =>
            !handle.IsInvalid && !handle.IsClosed;
    }
}

using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.SafeHandles;

namespace Neme.Extensions.FileSystem;

public readonly union FileSource(FileReference, FileSession, SafeFileHandle)
{
    public bool IsValid => this switch
    {
        FileReference reference => !reference.IsClosed,
        FileSession session => !session.IsClosed,
        SafeFileHandle handle => !handle.IsInvalid && !handle.IsClosed,
    };

    public bool CanSeek => this switch
    {
        FileReference reference => reference.Handle.CanSeek,
        FileSession session => session.Handle.CanSeek,
        SafeFileHandle handle => handle.CanSeek,
    };
}

using Microsoft.Win32.SafeHandles;

namespace Neme.Extensions.Internal;

internal sealed class NoopFileStream : FileStream
{
    public NoopFileStream(SafeFileHandle handle, FileAccess access)
    : base(handle, access)
    {
    }

    public override void Close()
    {
    }

    protected override void Dispose(bool disposing)
    {
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
    }
#endif
}

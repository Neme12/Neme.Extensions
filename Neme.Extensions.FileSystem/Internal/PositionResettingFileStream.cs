using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class PositionResettingFileStream : DelegatingFileStream
{
    private readonly long _originalPosition;

    public PositionResettingFileStream(FileStream fileStream) : base(fileStream)
    {
        _originalPosition = fileStream.SafeFileHandle.Position;
        fileStream.SafeFileHandle.Position = 0;
        Position = 0;
    }

    public override void Close()
    {
        SafeFileHandle.Position = _originalPosition;
        base.Close();
    }


#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        SafeFileHandle.Position = _originalPosition;
        await base.DisposeAsync();
    }
#endif
}

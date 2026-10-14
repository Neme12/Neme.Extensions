using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class PositionResettingFileStream : DelegatingFileStream
{
    private readonly long _originalPosition;

    public PositionResettingFileStream([OwnershipTransfer] FileStream fileStream) : base(fileStream)
    {
        _originalPosition = fileStream.SafeFileHandle.Position;
        fileStream.SafeFileHandle.Position = 0;
        Position = 0;
    }

    public override void Close()
    {
        try
        {
            SafeFileHandle.Position = _originalPosition;
        }
        finally
        {
            base.Close();
        }
    }


#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        try
        {
            SafeFileHandle.Position = _originalPosition;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
#endif
}

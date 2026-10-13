using Neme.Extensions.FileSystem.Resources;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class FileSessionFileStream : DelegatingFileStream
{
    private readonly FileSession _session;

    public FileSessionFileStream([OwnershipTransfer] FileSession session, [OwnershipTransfer] FileStream fileStream)
        : base(fileStream)
    {
        _session = session;
    }

    public override void Close()
    {
        try
        {
            base.Close();
        }
        finally
        {
            _session.Dispose();
        }
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        try
        {
            await base.DisposeAsync();
        }
        finally
        {
            _session.Dispose();
        }
    }
#endif
}

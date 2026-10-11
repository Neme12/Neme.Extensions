using Neme.Extensions.FileSystem.Resources;
using Neme.Extensions.IO;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class FileSessionFileStream : DelegatingFileStream
{
    private readonly FileSession _session;

    public FileSessionFileStream(FileSession session, FileStream fileStream) : base(fileStream)
    {
        _session = session;
    }

    public override void Close()
    {
        base.Close();
        _session.Dispose();
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        _session.Dispose();
    }
#endif
}

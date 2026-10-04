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
        _session.Dispose();
        base.Close();
    }

#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public override async ValueTask DisposeAsync()
    {
        _session.Dispose();
        await base.DisposeAsync();
    }
#endif
}

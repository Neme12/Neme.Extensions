using Neme.Extensions.Ownership;

namespace Neme.Extensions.IO;

public abstract class DelegatingFileStream : DelegatingFileStream<FileStream>
{
    protected DelegatingFileStream([OwnershipTransfer] FileStream innerStream)
        : base(innerStream)
    {
    }
}

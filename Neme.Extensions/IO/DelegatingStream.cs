using Neme.Extensions.Ownership;

namespace Neme.Extensions.IO;

public abstract class DelegatingStream : DelegatingStream<Stream>
{
    protected DelegatingStream([OwnershipTransfer] Stream innerStream)
        : base(innerStream)
    {
    }
}

using Neme.Extensions.Contracts;
using System.Buffers;

namespace Neme.Extensions.Buffers;

#pragma warning disable CA2265 // Do not compare Span<T> to 'null' or 'default'

public static partial class ArrayPoolExtensions
{
    extension<T>(ArrayPool<T> arrayPool)
    {
        public Lease<T> RentLease(int minimumLength, bool clearArray = false)
        {
            Require.ArgumentNotNull(arrayPool);
            Require.ArgumentNotNegative(minimumLength);

            return new Lease<T>(arrayPool, minimumLength, clearArray);
        }

        public LeaseOrBuffer<T> RentLeaseOrStackalloc(int minimumLength, Span<T> buffer, bool clearArray = false)
        {
            Require.ArgumentNotNull(arrayPool);
            Require.ArgumentNotNegative(minimumLength);

            return buffer != default
                ? new(arrayPool, buffer, clearArray)
                : new(arrayPool, minimumLength, clearArray);
        }
    }
}

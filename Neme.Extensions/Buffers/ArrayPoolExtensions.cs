using System.Buffers;
using System.Diagnostics;

namespace Neme.Extensions.Buffers;

#pragma warning disable CA2265 // Do not compare Span<T> to 'null' or 'default'

internal static partial class ArrayPoolExtensions
{
    extension<T>(ArrayPool<T> arrayPool)
    {
        public Lease<T> RentLease(int minimumLength, bool clearArray = false)
        {
            return new Lease<T>(arrayPool, minimumLength, clearArray);
        }

        public LeaseOrBuffer<T> RentLeaseOrStackalloc(int minimumLength, Span<T> buffer, bool clearArray = false)
        {
            return buffer != default
                ? new(buffer, clearArray)
                : new(arrayPool, minimumLength, clearArray);
        }
    }
}

using Neme.Extensions.Contracts;

namespace Neme.Extensions.Collections;

public static class ImmutableArrayBuilderExtensions
{
    public static void EnsureCapacity<T>(this ImmutableArray<T>.Builder builder, int capacity)
    {
        Require.ArgumentNotNull(builder);
        Require.ArgumentNotNegative(capacity);

        if (capacity > builder.Capacity)
        {
            int newCapacity = Math.Max(builder.Capacity * 2, capacity);
            builder.Capacity = newCapacity;
        }
    }
}

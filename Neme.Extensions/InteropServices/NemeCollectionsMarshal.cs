using Neme.Extensions.Collections;
using Neme.Extensions.Contracts;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class NemeCollectionsMarshal
{
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public static Span<T> AsSpan<T>(ref SmallImmutableArray<T>.Builder builder)
    {
        if (builder._items is { } items)
            return ImmutableCollectionsMarshalExtensions.AsSpan(items);

        Debug.AssertInRange(builder._count, 0, SmallImmutableArray<T>.InlineCapacity);
        return MemoryMarshal.CreateSpan(ref builder._item0!, builder._count);
    }

    public static ref readonly T ItemRef<T>(this in SmallImmutableArray<T>.Builder builder, int index)
    {
        Require.ArgumentInRange(index, 0, builder._count - 1);

        if (builder._items is { } items)
            return ref items.ItemRef(index);

        Debug.AssertInRange(builder._count, 1, SmallImmutableArray<T>.InlineCapacity);
        Debug.AssertInRange(index, 0, SmallImmutableArray<T>.InlineCapacity - 1);

        switch (index)
        {
            case 0:
                return ref builder._item0!;
            default:
                return ref builder._item1!;
        }
    }
#endif
}

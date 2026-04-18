namespace System.Collections.Immutable;

public static class ImmutableArrayBuilderPolyfill
{
    public static ImmutableArray<T> DrainToImmutable<T>(this ImmutableArray<T>.Builder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

#if NET8_0_OR_GREATER
        return builder.DrainToImmutable();
#else
        if (builder.Count == builder.Capacity)
        {
            return builder.MoveToImmutable();
        }
        else
        {
            var result = builder.ToImmutable();
            builder.Count = 0;
            builder.Capacity = 0;
            return result;
        }
#endif
    }
}

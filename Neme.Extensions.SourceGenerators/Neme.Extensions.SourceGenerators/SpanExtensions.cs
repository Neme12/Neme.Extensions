namespace Neme.Extensions.SourceGenerators;

internal static class SpanExtensions
{
    extension<T>(Span<T?> items)
        where T : class
    {
        public Span<T> RemoveNullValuesInPlace()
        {
            int seenIndex = 0;

            for (int index = 0; index < items.Length; index++)
            {
                if (items[index] is not null)
                {
                    items[seenIndex] = items[index!];
                    ++seenIndex;
                }
            }

            return items[..seenIndex]!;
        }
    }

    extension<T>(Span<T> items)
    {
        public Span<T> RemoveInPlace(Func<T, bool> predicate)
        {
            int seenIndex = 0;

            for (int index = 0; index < items.Length; index++)
            {
                if (!predicate(items[index]))
                {
                    items[seenIndex] = items[index];
                    ++seenIndex;
                }
            }

            return items[..seenIndex]!;
        }
    }

}

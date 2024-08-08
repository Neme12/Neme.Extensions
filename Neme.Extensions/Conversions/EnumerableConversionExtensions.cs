using System.Collections;

namespace Neme.Extensions.Conversions;

public static class EnumerableConversionExtensions
{
    public static IEnumerable AsNonGeneric<T>(this IEnumerable<T> enumerable) =>
        enumerable switch
        {
            NonGenericEnumerableWrapper<T> nonGenericEnumerableWrapper => nonGenericEnumerableWrapper.Enumerable,
            _ => enumerable,
        };

    public static IEnumerable<T> AsGeneric<T>(this IEnumerable enumerable) =>
        enumerable switch
        {
            IEnumerable<T> genericEnumerable => genericEnumerable,
            _ => new NonGenericEnumerableWrapper<T>(enumerable),
        };

    private sealed class NonGenericEnumerableWrapper<T>(IEnumerable enumerable) : IEnumerable<T>
    {
        public IEnumerable Enumerable => enumerable;

        public IEnumerator<T> GetEnumerator() =>
            enumerable.GetEnumerator().AsGeneric<T>();

        IEnumerator IEnumerable.GetEnumerator() =>
            enumerable.GetEnumerator();
    }
}

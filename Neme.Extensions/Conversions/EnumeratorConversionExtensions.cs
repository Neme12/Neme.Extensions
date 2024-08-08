using System.Collections;

namespace Neme.Extensions.Conversions;

public static class EnumeratorConversionExtensions
{
    public static IEnumerator AsNonGeneric<T>(this IEnumerator<T> enumerator) =>
        enumerator;

    public static IEnumerator<T> AsGeneric<T>(this IEnumerator enumerator) =>
        enumerator switch
        {
            IEnumerator<T> genericEnumerator => genericEnumerator,
            _ => new NonGenericEnumeratorWrapper<T>(enumerator),
        };

    private sealed class NonGenericEnumeratorWrapper<T>(IEnumerator enumerator) : IEnumerator<T>
    {
        public T Current => (T)enumerator.Current;

        object IEnumerator.Current => enumerator.Current;

        public bool MoveNext() =>
            enumerator.MoveNext();

        public void Reset() =>
            enumerator.Reset();

        public void Dispose() =>
            enumerator.Reset();
    }
}

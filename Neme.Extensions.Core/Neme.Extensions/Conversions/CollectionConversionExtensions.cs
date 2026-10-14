using System.Collections;

namespace Neme.Extensions.Conversions;

public static class CollectionConversionExtensions
{
    public static ICollection AsNonGeneric<T>(this ICollection<T> collection) =>
        collection switch
        {
            NonGenericCollectionWrapper<T> nonGenericCollectionWrapper => nonGenericCollectionWrapper.Collection,
            ICollection nonGenericCollection => nonGenericCollection,
            _ => new GenericCollectionWrapper<T>(collection),
        };

    public static ICollection<T> AsGeneric<T>(this ICollection collection) =>
        collection switch
        {
            GenericCollectionWrapper<T> genericCollectionWrapper => genericCollectionWrapper.Collection,
            ICollection<T> genericCollection => genericCollection,
            _ => new NonGenericCollectionWrapper<T>(collection),
        };

    private sealed class NonGenericCollectionWrapper<T>(ICollection collection) : ICollection<T>
    {
        public ICollection Collection => collection;

        public int Count =>
            collection.Count;

        public bool IsReadOnly =>
            true;

        public void Add(T item) =>
            ThrowHelper.ThrowNotSupported_ReadOnlyCollection();

        public bool Remove(T item)
        {
            ThrowHelper.ThrowNotSupported_ReadOnlyCollection();
            return default;
        }

        public void Clear() =>
            ThrowHelper.ThrowNotSupported_ReadOnlyCollection();

        public bool Contains(T item) =>
            ((IEnumerable)collection).AsGeneric<T>().Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            collection.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            collection.GetEnumerator().AsGeneric<T>();

        IEnumerator IEnumerable.GetEnumerator() =>
            collection.GetEnumerator();
    }

    private sealed class GenericCollectionWrapper<T>(ICollection<T> collection) : ICollection
    {
        public ICollection<T> Collection => collection;

        public int Count =>
            collection.Count;

        public object SyncRoot =>
            collection;

        public bool IsSynchronized =>
            false;

        public void CopyTo(Array array, int index) =>
            collection.CopyTo((T[])array, index);

        public IEnumerator GetEnumerator() =>
            collection.GetEnumerator();
    }
}

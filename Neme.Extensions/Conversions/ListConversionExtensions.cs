using System.Collections;

namespace Neme.Extensions.Conversions;

public static class ListConversionExtensions
{
    public static IList AsNonGeneric<T>(this IList<T> list) =>
        list switch
        {
            NonGenericListWrapper<T> nonGenericListWrapper => nonGenericListWrapper.List,
            IList nonGenericList => nonGenericList,
            _ => new GenericListWrapper<T>(list),
        };

    public static IList<T> AsGeneric<T>(this IList list) =>
        list switch
        {
            GenericListWrapper<T> genericListWrapper => genericListWrapper.List,
            IList<T> genericList => genericList,
            _ => new NonGenericListWrapper<T>(list),
        };

    private sealed class NonGenericListWrapper<T>(IList list) : IList<T>
    {
        public IList List => list;

        public int Count =>
            list.Count;

        public bool IsReadOnly =>
            list.IsReadOnly;

        public T this[int index]
        {
            get => (T)list[index]!;
            set => list[index] = value;
        }

        public void Add(T item) =>
            list.Add(item);

        public void Insert(int index, T item) =>
            list.Insert(index, item);

        public bool Remove(T item)
        {
            var count = list.Count;
            list.Remove(item);
            return list.Count < count;
        }

        public void RemoveAt(int index) =>
            list.RemoveAt(index);

        public void Clear() =>
            list.Clear();

        public bool Contains(T item) =>
            list.Contains(item);

        public int IndexOf(T item) =>
            list.IndexOf(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            list.GetEnumerator().AsGeneric<T>();

        IEnumerator IEnumerable.GetEnumerator() =>
            list.GetEnumerator();
    }

    private sealed class GenericListWrapper<T>(IList<T> list) : IList
    {
        public IList<T> List => list;

        public int Count =>
            list.Count;

        public object SyncRoot =>
            list;

        public bool IsSynchronized =>
            false;

        public bool IsReadOnly =>
            list.IsReadOnly;

        public bool IsFixedSize =>
            list.IsReadOnly || list is Array;

        public object? this[int index]
        {
            get => list[index];
            set => list[index] = (T)value!;
        }

        public int Add(object? value)
        {
            var count = list.Count;
            list.Add((T)value!);
            return count;
        }

        public void Insert(int index, object? value) =>
            list.Insert(index, (T)value!);

        public void Remove(object? value) =>
            list.Remove((T)value!);

        public void RemoveAt(int index) =>
            list.RemoveAt(index);

        public void Clear() =>
            list.Clear();

        public bool Contains(object? value) =>
            list.Contains((T)value!);

        public int IndexOf(object? value) =>
            list.IndexOf((T)value!);

        public void CopyTo(Array array, int index) =>
            list.CopyTo((T[])array, index);

        public IEnumerator GetEnumerator() =>
            list.GetEnumerator();
    }
}

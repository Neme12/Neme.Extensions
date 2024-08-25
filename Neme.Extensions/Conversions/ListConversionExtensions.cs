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

    public static IReadOnlyList<T> AsIReadOnlyList<T>(this IList<T> list) =>
        list switch
        {
            GenericReadOnlyListWrapper<T> genericReadOnlyListWrapper => genericReadOnlyListWrapper.List,
            IReadOnlyList<T> genericReadOnlyList => genericReadOnlyList,
            _ => new GenericListWrapper<T>(list),
        };

    public static IList<T> AsIList<T>(this IReadOnlyList<T> list) =>
        list switch
        {
            GenericListWrapper<T> genericListWrapper => genericListWrapper.List,
            IList<T> genericList => genericList,
            _ => new GenericReadOnlyListWrapper<T>(list),
        };

    private sealed class NonGenericListWrapper<T>(IList list) : IList<T>, IReadOnlyList<T>
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

    private sealed class GenericListWrapper<T>(IList<T> list) : IReadOnlyList<T>, IList
    {
        public IList<T> List => list;

        public int Count =>
            list.Count;

        object ICollection.SyncRoot =>
            list;

        bool ICollection.IsSynchronized =>
            false;

        bool IList.IsReadOnly =>
            list.IsReadOnly;

        bool IList.IsFixedSize =>
            list.IsReadOnly || list is Array;

        public T this[int index] =>
            list[index];

        object? IList.this[int index]
        {
            get => list[index];
            set => list[index] = (T)value!;
        }

        int IList.Add(object? value)
        {
            var count = list.Count;
            list.Add((T)value!);
            return count;
        }

        void IList.Insert(int index, object? value) =>
            list.Insert(index, (T)value!);

        void IList.Remove(object? value) =>
            list.Remove((T)value!);

        void IList.RemoveAt(int index) =>
            list.RemoveAt(index);

        void IList.Clear() =>
            list.Clear();

        bool IList.Contains(object? value) =>
            list.Contains((T)value!);

        int IList.IndexOf(object? value) =>
            list.IndexOf((T)value!);

        void ICollection.CopyTo(Array array, int index) =>
            list.CopyTo((T[])array, index);

        public IEnumerator<T> GetEnumerator() =>
            list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            list.GetEnumerator();
    }

    private sealed class GenericReadOnlyListWrapper<T>(IReadOnlyList<T> list) : IList<T>, IList
    {
        public IReadOnlyList<T> List => list;

        public int Count =>
            list.Count;

        object ICollection.SyncRoot =>
            list;

        bool ICollection.IsSynchronized =>
            false;

        public bool IsReadOnly =>
            true;

        bool IList.IsFixedSize =>
            true;

        public T this[int index]
        {
            get => list[index];
            set => throw new NotSupportedException();
        }

        object? IList.this[int index]
        {
            get => list[index];
            set => throw new NotSupportedException();
        }

        public void Add(T item) =>
            throw new NotSupportedException();

        int IList.Add(object? value) =>
            throw new NotSupportedException();

        public void Insert(int index, T item) =>
            throw new NotSupportedException();

        void IList.Insert(int index, object? value) =>
            throw new NotSupportedException();

        public bool Remove(T item) =>
            throw new NotSupportedException();

        void IList.Remove(object? value) =>
            throw new NotSupportedException();

        public void RemoveAt(int index) =>
            throw new NotSupportedException();

        void IList.RemoveAt(int index) =>
            throw new NotSupportedException();

        public void Clear() =>
            throw new NotSupportedException();

        void IList.Clear() =>
            throw new NotSupportedException();

        public bool Contains(T item) =>
            list.Contains(item);

        bool IList.Contains(object? value) =>
            list.Contains((T)value!);

        public int IndexOf(T item)
        {
            int i = 0;

            foreach (var element in list)
            {
                if (EqualityComparer<T>.Default.Equals(element, item))
                    return i;

                ++i;
            }

            return -1;
        }

        int IList.IndexOf(object? value) =>
            IndexOf((T)value!);

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = arrayIndex, end = arrayIndex + list.Count; i < end; ++i)
                array[i] = list[i];
        }

        void ICollection.CopyTo(Array array, int index) =>
            CopyTo((T[])array, index);

        public IEnumerator<T> GetEnumerator() =>
            list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            list.GetEnumerator();
    }
}

using Neme.Extensions.Tests.Utilities;
using System.Collections;
using System.Collections.ObjectModel;
using Xunit.Sdk;

namespace Neme.Extensions.Conversions.Tests;

public sealed class ListConversionExtensionsTests
{
    [Fact]
    public void TestAsNonGeneric_Same()
    {
        var list = (IList<int>)new List<int> { 1, 2, 3 };
        var asNonGeneric = list.AsNonGeneric();
        Assert.Same(list, asNonGeneric);
    }

    [Fact]
    public void TestAsNonGeneric_ReadOnlyCollectionWrapper()
    {
        var list = new GenericList<int>(new ReadOnlyCollection<int>([1, 2, 3]));
        var asNonGeneric = list.AsNonGeneric();
        Assert.NotSame(list, asNonGeneric);
        Assert.Equal(3, asNonGeneric.Count);
        Assert.Same(list, asNonGeneric.SyncRoot);
        Assert.False(asNonGeneric.IsSynchronized);
        Assert.True(asNonGeneric.IsReadOnly);
        Assert.True(asNonGeneric.IsFixedSize);
        Assert.Equal([1, 2, 3], asNonGeneric.Cast<int>());

        Assert.Equal(2, asNonGeneric[1]);

        Assert.True(asNonGeneric.Contains(3));
        Assert.Equal(2, asNonGeneric.IndexOf(3));

        Assert.False(asNonGeneric.Contains(4));
        Assert.Equal(-1, asNonGeneric.IndexOf(4));

        AssertThrows.NotSupported_ReadOnlyCollection(() => asNonGeneric[0] = 0);
        AssertThrows.NotSupported_ReadOnlyCollection(() => asNonGeneric.Add(4));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asNonGeneric.Insert(1, 1));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asNonGeneric.Remove(3));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asNonGeneric.RemoveAt(2));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asNonGeneric.Clear());

        var array = new int[3];
        asNonGeneric.CopyTo(array, 0);
        Assert.Equal((int[])[1, 2, 3], array);
    }

    [Fact]
    public void TestAsNonGeneric_ListWrapper()
    {
        var list = new GenericList<int>([1, 2, 3]);
        var asNonGeneric = list.AsNonGeneric();
        Assert.NotSame(list, asNonGeneric);
        Assert.Equal(3, asNonGeneric.Count);
        Assert.Same(list, asNonGeneric.SyncRoot);
        Assert.False(asNonGeneric.IsSynchronized);
        Assert.False(asNonGeneric.IsReadOnly);
        Assert.False(asNonGeneric.IsFixedSize);
        Assert.Equal([1, 2, 3], asNonGeneric.Cast<int>());

        Assert.Equal(2, asNonGeneric[1]);

        Assert.True(asNonGeneric.Contains(3));
        Assert.Equal(2, asNonGeneric.IndexOf(3));

        Assert.False(asNonGeneric.Contains(4));
        Assert.Equal(-1, asNonGeneric.IndexOf(4));

        asNonGeneric[0] = 0;
        Assert.Equal(0, asNonGeneric[0]);
        Assert.Equal([0, 2, 3], asNonGeneric.Cast<int>());
        Assert.Equal([0, 2, 3], list);

        Assert.Equal(3, asNonGeneric.Add(4));
        Assert.Equal(4, asNonGeneric[3]);
        Assert.Equal([0, 2, 3, 4], asNonGeneric.Cast<int>());
        Assert.Equal([0, 2, 3, 4], list);

        asNonGeneric.Insert(1, 1);
        Assert.Equal(1, asNonGeneric[1]);
        Assert.Equal([0, 1, 2, 3, 4], asNonGeneric.Cast<int>());
        Assert.Equal([0, 1, 2, 3, 4], list);

        asNonGeneric.Remove(3);
        Assert.Equal([0, 1, 2, 4], asNonGeneric.Cast<int>());
        Assert.Equal([0, 1, 2, 4], list);

        asNonGeneric.RemoveAt(2);
        Assert.Equal([0, 1, 4], asNonGeneric.Cast<int>());
        Assert.Equal([0, 1, 4], list);

        var array = new int[3];
        asNonGeneric.CopyTo(array, 0);
        Assert.Equal((int[])[0, 1, 4], array);

        asNonGeneric.Clear();
        Assert.Equal([], asNonGeneric.Cast<int>());
        Assert.Equal([], list);
    }

    [Fact]
    public void TestAsNonGeneric_GenericWrapperUnwrapped()
    {
        var list = new NonGenericList((List<int>)[1, 2, 3]);
        var asGeneric = list.AsGeneric<int>();
        var asNonGeneric = asGeneric.AsNonGeneric();
        Assert.Same(list, asNonGeneric);
    }

    [Fact]
    public void TestAsGeneric_Same()
    {
        var list = (IList)new List<int> { 1, 2, 3 };
        var asGeneric = list.AsGeneric<int>();
        Assert.Same(list, asGeneric);
    }

    [Fact]
    public void TestAsGeneric_WrongType()
    {
        var list = (IList)new List<int> { 1, 2, 3 };
        var asGeneric = list.AsGeneric<string>();

        using var enumerator = asGeneric.GetEnumerator();
        enumerator.MoveNext();
        Assert.Throws<InvalidCastException>(() => enumerator.Current);
    }

    [Fact]
    public void TestAsGeneric_ReadOnlyCollectionWrapper()
    {
        var list = new NonGenericList(new ReadOnlyCollection<int>([1, 2, 3]));
        var asGeneric = list.AsGeneric<int>();
        Assert.NotSame(list, asGeneric);
        Assert.Equal(3, asGeneric.Count);
        Assert.True(asGeneric.IsReadOnly);
        Assert.Equal([1, 2, 3], asGeneric);

        Assert.Equal(2, asGeneric[1]);

        Assert.True(asGeneric.Contains(3));
        Assert.Equal(2, asGeneric.IndexOf(3));

        Assert.False(asGeneric.Contains(4));
        Assert.Equal(-1, asGeneric.IndexOf(4));

        AssertThrows.NotSupported_ReadOnlyCollection(() => asGeneric[0] = 0);
        AssertThrows.NotSupported_ReadOnlyCollection(() => asGeneric.Add(4));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asGeneric.Insert(1, 1));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asGeneric.Remove(3));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asGeneric.RemoveAt(2));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asGeneric.Clear());

        var array = new int[3];
        asGeneric.CopyTo(array, 0);
        Assert.Equal((int[])[1, 2, 3], array);
    }

    [Fact]
    public void TestAsGeneric_ListWrapper()
    {
        var list = new NonGenericList((List<int>)[1, 2, 3]);
        var asGeneric = list.AsGeneric<int>();
        Assert.NotSame(list, asGeneric);
        Assert.Equal(3, asGeneric.Count);
        Assert.False(asGeneric.IsReadOnly);
        Assert.Equal([1, 2, 3], asGeneric);

        Assert.Equal(2, asGeneric[1]);

        Assert.True(asGeneric.Contains(3));
        Assert.Equal(2, asGeneric.IndexOf(3));

        Assert.False(asGeneric.Contains(4));
        Assert.Equal(-1, asGeneric.IndexOf(4));

        asGeneric[0] = 0;
        Assert.Equal(0, asGeneric[0]);
        Assert.Equal([0, 2, 3], asGeneric);
        Assert.Equal([0, 2, 3], list.Cast<int>());

        asGeneric.Add(4);
        Assert.Equal(4, asGeneric[3]);
        Assert.Equal([0, 2, 3, 4], asGeneric);
        Assert.Equal([0, 2, 3, 4], list.Cast<int>());

        asGeneric.Insert(1, 1);
        Assert.Equal(1, asGeneric[1]);
        Assert.Equal([0, 1, 2, 3, 4], asGeneric);
        Assert.Equal([0, 1, 2, 3, 4], list.Cast<int>());

        Assert.True(asGeneric.Remove(3));
        Assert.Equal([0, 1, 2, 4], asGeneric);
        Assert.Equal([0, 1, 2, 4], list.Cast<int>());

        asGeneric.RemoveAt(2);
        Assert.Equal([0, 1, 4], asGeneric);
        Assert.Equal([0, 1, 4], list.Cast<int>());

        var array = new int[3];
        asGeneric.CopyTo(array, 0);
        Assert.Equal((int[])[0, 1, 4], array);

        asGeneric.Clear();
        Assert.Equal([], asGeneric);
        Assert.Equal([], list.Cast<int>());
    }

    [Fact]
    public void TestAsGeneric_NonGenericWrapperUnwrapped()
    {
        var list = new GenericList<int>([1, 2, 3]);
        var asNonGeneric = list.AsNonGeneric();
        var asGeneric = asNonGeneric.AsGeneric<int>();
        Assert.Same(list, asGeneric);
    }

    [Fact]
    public void TestAsIReadOnlyList_Same()
    {
        var list = (IList<int>)new List<int> { 1, 2, 3 };
        var asIReadOnlyList = list.AsIReadOnlyList();
        Assert.Same(list, asIReadOnlyList);
    }

    [Fact]
    public void TestAsIReadOnlyList_ListWrapper()
    {
        var list = new GenericList<int>([1, 2, 3]);
        var asIReadOnlyList = list.AsIReadOnlyList();
        Assert.NotSame(list, asIReadOnlyList);
        Assert.Equal(3, asIReadOnlyList.Count);
        Assert.Equal([1, 2, 3], asIReadOnlyList);

        Assert.Equal(2, asIReadOnlyList[1]);
    }

    [Fact]
    public void TestAsIReadOnlyList_GenericListWrapperUnwrapped()
    {
        var list = new GenericList<int>([1, 2, 3]);
        var asIReadOnlyList = list.AsIReadOnlyList();
        var asIList = asIReadOnlyList.AsIList();
        Assert.Same(list, asIList);
    }

    [Fact]
    public void TestAsIList_Same()
    {
        var list = (IReadOnlyList<int>)new List<int> { 1, 2, 3 };
        var asIList = list.AsIList();
        Assert.Same(list, asIList);
    }

    [Fact]
    public void TestAsIList_ListWrapper()
    {
        var list = new GenericReadOnlyList<int>([1, 2, 3]);
        var asIList = list.AsIList();
        Assert.NotSame(list, asIList);
        Assert.Equal(3, asIList.Count);
        Assert.True(asIList.IsReadOnly);
        Assert.Equal([1, 2, 3], asIList);

        Assert.Equal(2, asIList[1]);

        Assert.True(asIList.Contains(3));
        Assert.Equal(2, asIList.IndexOf(3));

        Assert.False(asIList.Contains(4));
        Assert.Equal(-1, asIList.IndexOf(4));

        AssertThrows.NotSupported_ReadOnlyCollection(() => asIList[0] = 0);
        AssertThrows.NotSupported_ReadOnlyCollection(() => asIList.Add(4));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asIList.Insert(1, 1));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asIList.Remove(3));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asIList.RemoveAt(2));
        AssertThrows.NotSupported_ReadOnlyCollection(() => asIList.Clear());

        var array = new int[3];
        asIList.CopyTo(array, 0);
        Assert.Equal((int[])[1, 2, 3], array);
    }

    [Fact]
    public void TestAsIReadOnlyList_GenericReadOnlyListWrapperUnwrapped()
    {
        var list = new GenericReadOnlyList<int>([1, 2, 3]);
        var asIList = list.AsIList();
        var asIReadOnlyList = asIList.AsIReadOnlyList();
        Assert.Same(list, asIReadOnlyList);
    }

    // A wrapper class is needed to ensure that is only implements IList and not other interfaces.
    private sealed class NonGenericList(IList list) : IList
    {
        public int Count =>
            list.Count;

        public bool IsSynchronized =>
            list.IsSynchronized;

        public object SyncRoot =>
            list.SyncRoot;

        public bool IsReadOnly =>
            list.IsReadOnly;

        public bool IsFixedSize =>
            list.IsFixedSize;

        public object? this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public int Add(object? value) =>
            list.Add(value);

        public void Insert(int index, object? value) =>
            list.Insert(index, value);

        public void Remove(object? value) =>
            list.Remove(value);

        public void RemoveAt(int index) =>
            list.RemoveAt(index);

        public void Clear() =>
            list.Clear();

        public bool Contains(object? value) =>
            list.Contains(value);

        public int IndexOf(object? value) =>
            list.IndexOf(value);

        public void CopyTo(Array array, int index) =>
            list.CopyTo(array, index);

        public IEnumerator GetEnumerator() =>
            list.GetEnumerator();
    }

    // A wrapper class is needed to ensure that is only implements IList<T> and not other interfaces.
    private sealed class GenericList<T>(IList<T> list) : IList<T>
    {
        public int Count =>
            list.Count;

        public bool IsReadOnly =>
            list.IsReadOnly;

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public void Add(T item) =>
            list.Add(item);

        public void Insert(int index, T item) =>
            list.Insert(index, item);

        public bool Remove(T item) =>
            list.Remove(item);

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
            list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)list).GetEnumerator();
    }

    // A wrapper class is needed to ensure that is only implements IReadOnlyList<T> and not other interfaces.
    private sealed class GenericReadOnlyList<T>(IReadOnlyList<T> list) : IReadOnlyList<T>
    {
        public int Count =>
            list.Count;

        public T this[int index] =>
            list[index];

        public IEnumerator<T> GetEnumerator() =>
            list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)list).GetEnumerator();
    }
}

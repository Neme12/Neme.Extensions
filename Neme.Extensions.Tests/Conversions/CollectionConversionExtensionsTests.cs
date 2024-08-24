using System.Collections;
using Xunit.Sdk;

namespace Neme.Extensions.Conversions.Tests;

public sealed class CollectionConversionExtensionsTests
{
    [Fact]
    public void TestAsNonGeneric_Same()
    {
        var collection = (ICollection<int>)new List<int> { 1, 2, 3 };
        var asNonGeneric = collection.AsNonGeneric();
        Assert.Same(collection, asNonGeneric);
    }

    [Fact]
    public void TestAsNonGeneric_Wrapper()
    {
        var collection = new GenericCollection<int>([1, 2, 3]);
        var asNonGeneric = collection.AsNonGeneric();
        Assert.NotSame(collection, asNonGeneric);
        Assert.Equal(3, asNonGeneric.Count);
        Assert.False(asNonGeneric.IsSynchronized);
        Assert.Same(asNonGeneric, asNonGeneric.SyncRoot);
        Assert.Equal([1, 2, 3], asNonGeneric.Cast<int>());

        var array = new int[3];
        asNonGeneric.CopyTo(array, 0);
        Assert.Equal((int[])[1, 2, 3], array);
    }

    [Fact]
    public void TestAsNonGeneric_GenericWrapperUnwrapped()
    {
        var collection = new NonGenericCollection<int>([1, 2, 3]);
        var asGeneric = collection.AsGeneric<int>();
        var asNonGeneric = asGeneric.AsNonGeneric();
        Assert.Same(collection, asNonGeneric);
    }

    [Fact]
    public void TestAsGeneric_Same()
    {
        var collection = (ICollection)new List<int> { 1, 2, 3 };
        var asGeneric = collection.AsGeneric<int>();
        Assert.Same(collection, asGeneric);
    }

    [Fact]
    public void TestAsGeneric_WrongType()
    {
        var collection = (ICollection)new List<int> { 1, 2, 3 };
        var asGeneric = collection.AsGeneric<string>();

        using var enumerator = asGeneric.GetEnumerator();
        enumerator.MoveNext();
        Assert.Throws<InvalidCastException>(() => enumerator.Current);
    }

    [Fact]
    public void TestAsGeneric_Wrapper()
    {
        var collection = new NonGenericCollection<int>([1, 2, 3]);
        var asGeneric = collection.AsGeneric<int>();
        Assert.NotSame(collection, asGeneric);
        Assert.Equal(3, asGeneric.Count);
        Assert.True(asGeneric.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => asGeneric.Add(1));
        Assert.Throws<NotSupportedException>(() => asGeneric.Remove(1));
        Assert.Throws<NotSupportedException>(() => asGeneric.Clear());
        Assert.True(asGeneric.Contains(1));
        Assert.False(asGeneric.Contains(0));
        Assert.Equal([1, 2, 3], asGeneric);

        var array = new int[3];
        asGeneric.CopyTo(array, 0);
        Assert.Equal((int[])[1, 2, 3], array);
    }

    [Fact]
    public void TestAsGeneric_NonGenericWrapperUnwrapped()
    {
        var collection = new GenericCollection<int>([1, 2, 3]);
        var asNonGeneric = collection.AsNonGeneric();
        var asGeneric = asNonGeneric.AsGeneric<int>();
        Assert.Same(collection, asGeneric);
    }

    private sealed class NonGenericCollection<T>(List<T> list) : ICollection
    {
        public int Count =>
            ((ICollection)list).Count;

        public bool IsSynchronized =>
            ((ICollection)list).IsSynchronized;

        public object SyncRoot =>
            ((ICollection)list).SyncRoot;

        public void CopyTo(Array array, int index) =>
            ((ICollection)list).CopyTo(array, index);

        public IEnumerator GetEnumerator() =>
            ((IEnumerable)list).GetEnumerator();
    }

    private sealed class GenericCollection<T>(List<T> list) : ICollection<T>
    {
        public int Count =>
            ((ICollection<T>)list).Count;

        public bool IsReadOnly =>
            ((ICollection<T>)list).IsReadOnly;

        public void Add(T item) =>
            ((ICollection<T>)list).Add(item);

        public bool Remove(T item) =>
            ((ICollection<T>)list).Remove(item);

        public void Clear() =>
            ((ICollection<T>)list).Clear();

        public bool Contains(T item) =>
            ((ICollection<T>)list).Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            ((ICollection<T>)list).CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            ((IEnumerable<T>)list).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)list).GetEnumerator();
    }
}

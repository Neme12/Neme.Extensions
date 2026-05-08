using System.Runtime.CompilerServices;
using Neme.Extensions.Contracts;
using Neme.Extensions.Internal;

namespace Neme.Extensions.InteropServices;

public static class CollectionsMarshalExtensions
{
    public static Memory<T> AsMemory<T>(List<T>? list)
    {
        if (list is null)
            return default;

        var array = ListShadow<T>.From(list)._items;
        return array.AsMemory(0, list.Count);
    }

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    private sealed class ListShadow<T>
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
#pragma warning disable CS0649 // Field 'CollectionsMarshalEx.ListShadow<T>._size' is never assigned to, and will always have its default value 0
        public T[] _items = null!;
        public int _size;
        public int _version;
#pragma warning restore CS0649 // Field 'CollectionsMarshalEx.ListShadow<T>._size' is never assigned to, and will always have its default value 0

        private ListShadow()
        {
        }

        public static ListShadow<T> From(List<T> list)
        {
            Assert.True(ShadowTypeUtilities.MatchesShadow(typeof(List<T>), typeof(ListShadow<T>)));
            return Unsafe.As<ListShadow<T>>(list);
        }
    }
}

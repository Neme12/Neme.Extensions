using Neme.Extensions.Contracts;
using Neme.Extensions.Reflection;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class CollectionsMarshalExtensions
{
    // For some reason, UnsafeAccessor doesn't work on .NET 8:
    // System.BadImageFormatException : Invalid usage of UnsafeAccessorAttribute.
#if !NET9_0_OR_GREATER
    private static readonly FieldInfo _itemsField = typeof(List<>)
        .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding).NotNull();
#endif

    private static class Accessors<T>
    {
#if NET9_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
        public static extern ref T[] GetItems(List<T> list);
#else
        private static readonly FieldInfo _itemsField =
            typeof(List<T>).GetFieldWithSameMetadataDefinitionAs(CollectionsMarshalExtensions._itemsField);

        public static readonly GetItemsDelegate GetItems = _itemsField.CreateGetDelegate<GetItemsDelegate>();

        public delegate T[] GetItemsDelegate(List<T> list);
#endif
    }

    extension(CollectionsMarshal)
    {
        public static Memory<T> AsMemory<T>(List<T>? list)
        {
            if (list is null)
                return default;

            var array = Accessors<T>.GetItems(list);
            return array.AsMemory(0, list.Count);
        }
    }
}

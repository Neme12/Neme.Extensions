using Neme.Extensions.Contracts;
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
        public static readonly FieldInfo ItemsField =
            typeof(List<T>).GetFieldWithSameMetadataDefinitionAs(CollectionsMarshalExtensions._itemsField);
#endif
    }

    extension(CollectionsMarshal)
    {
        public static Memory<T> AsMemory<T>(List<T>? list)
        {
            if (list is null)
                return default;

#if NET9_0_OR_GREATER
            var array = Accessors<T>.GetItems(list);
            return array.AsMemory(0, list.Count);
#else
            var array = (T[])Accessors<T>.ItemsField.GetValue(list)!;
            return array.AsMemory(0, list.Count);
#endif
        }
    }
}

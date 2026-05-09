using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class CollectionsMarshalExtensions
{
#if !NET8_0_OR_GREATER
    private static readonly FieldInfo? _itemsField = typeof(List<>)
        .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding);
#endif

    private static class Accessors<T>
    {
#if NET8_0_OR_GREATER
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
        public static extern ref T[] GetItems(List<T> list);
#else
        public static readonly FieldInfo? ItemsField =
            _itemsField is null
                ? null
                : (FieldInfo)typeof(List<T>).GetMemberWithSameMetadataDefinitionAs(_itemsField);
#endif
    }

    extension(CollectionsMarshal)
    {
        public static Memory<T> AsMemory<T>(List<T>? list)
        {
            if (list is null)
                return default;

#if NET8_0_OR_GREATER
            return Accessors<T>.GetItems(list).AsMemory(0..list.Count);
#else
            if (Accessors<T>.ItemsField is not { } itemsField)
                throw new PlatformNotSupportedException();

            var array = (T[])itemsField.GetValue(list)!;
            return array.AsMemory(0, list.Count);
#endif
        }
    }
}

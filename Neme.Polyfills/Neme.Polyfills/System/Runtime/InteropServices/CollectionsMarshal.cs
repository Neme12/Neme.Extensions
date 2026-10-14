using System.Reflection;

namespace System.Runtime.InteropServices;

#if !NET5_0_OR_GREATER
public static class CollectionsMarshal
{
    private static readonly FieldInfo? _itemsField = typeof(List<>)
        .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding);

    private static class Accessors<T>
    {
        public static readonly FieldInfo? ItemsField =
            _itemsField is null
                ? null
                : (FieldInfo)typeof(List<T>).GetMemberWithSameMetadataDefinitionAs(_itemsField);
    }

    public static Span<T> AsSpan<T>(List<T>? list)
    {
        if (list is null)
            return default;

        if (Accessors<T>.ItemsField is not { } itemsField)
            throw new PlatformNotSupportedException();

        var array = (T[])itemsField.GetValue(list)!;
        return array.AsSpan(0, list.Count);
    }
}
#endif

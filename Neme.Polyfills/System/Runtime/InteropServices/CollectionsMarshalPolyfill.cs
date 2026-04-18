using System.Reflection;

namespace System.Runtime.InteropServices;

public static class CollectionsMarshalPolyfill
{
#if !NET5_0_OR_GREATER
    private static readonly FieldInfo? _itemsField = typeof(List<>)
        .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding);
#endif

    public static Span<T> AsSpan<T>(List<T>? list)
    {
#if NET5_0_OR_GREATER
        return CollectionsMarshal.AsSpan(list);
#else
        if (_itemsField is not { } itemsField)
            throw new PlatformNotSupportedException();

        if (list is null)
            return default;

        var field = (FieldInfo)typeof(List<T>).GetMemberWithSameMetadataDefinitionAs(itemsField);
        var array = (T[])field.GetValue(list);
        return array.AsSpan(0, list.Count);
#endif
    }
}

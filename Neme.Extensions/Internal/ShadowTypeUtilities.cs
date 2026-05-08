using System.Diagnostics;
using System.Reflection;

namespace Neme.Extensions.Internal;

internal static class ShadowTypeUtilities
{
    public static bool MatchesShadow(Type actualType, Type shadowType)
    {
        if (actualType is null)
            throw new ArgumentNullException(nameof(actualType));

        if (shadowType is null)
            throw new ArgumentNullException(nameof(shadowType));

        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        var actualFields = actualType
            .GetFields(bindingFlags)
            .OrderBy(x => x.MetadataToken)
            .ToImmutableArray();

        var shadowFields = shadowType
            .GetFields(bindingFlags)
            .OrderBy(x => x.MetadataToken)
            .ToImmutableArray();

        return FieldsEqual(actualFields, shadowFields);
    }

    private static bool FieldsEqual(ImmutableArray<FieldInfo> actualFields, ImmutableArray<FieldInfo> shadowFields)
    {
        Debug.Assert(actualFields != default);
        Debug.Assert(shadowFields != default);

        for (int i = 0, length = shadowFields.Length; i < length; ++i)
        {
            if (!FieldEquals(shadowFields[i], actualFields[i]))
                return false;
        }

        return true;

        static bool FieldEquals(FieldInfo field1, FieldInfo field2)
        {
            Debug.Assert(field1 is not null);
            Debug.Assert(field2 is not null);

            return field1!.Name == field2!.Name &&
            field1.FieldType == field2.FieldType &&
            field1.IsStatic == field2.IsStatic &&
            field1.IsInitOnly == field2.IsInitOnly;
        }
    }
}

using System.Reflection;

namespace System;

public static class TypePolyfill
{
    public static MethodInfo? GetMethod(
        this Type type,
        string name,
        BindingFlags bindingAttr,
        Type[] types)
    {
#if NET6_0_OR_GREATER
        return type.GetMethod(name, bindingAttr, types);
#else
        return type.GetMethod(name, bindingAttr, binder: null, types, modifiers: null);
#endif
    }

    public static MemberInfo GetMemberWithSameMetadataDefinitionAs(this Type type, MemberInfo member)
    {
#if NET6_0_OR_GREATER
        return type.GetMemberWithSameMetadataDefinitionAs(member);
#else
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (member is null)
        {
            throw new ArgumentNullException(nameof(member));
        }

        const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        foreach (var myMemberInfo in type.GetMembers(all))
        {
            if (myMemberInfo.HasSameMetadataDefinitionAs(member))
                return myMemberInfo;
        }

        throw new ArgumentException(message: null, paramName: nameof(member));
#endif
    }
}

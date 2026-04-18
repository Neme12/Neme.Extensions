namespace System.Reflection;

public static class MemberInfoPolyfill
{
    public static bool HasSameMetadataDefinitionAs(this MemberInfo member, MemberInfo other)
    {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return member.HasSameMetadataDefinitionAs(other);
#else
        if (member is null)
            throw new ArgumentNullException(nameof(member));

        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (member.MetadataToken != other.MetadataToken)
            return false;

        if (!member.Module.Equals(other.Module))
            return false;

        return true;
#endif
    }
}

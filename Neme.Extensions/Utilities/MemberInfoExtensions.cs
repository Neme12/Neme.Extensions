using System.Reflection;

namespace Neme.Extensions.Utilities;

internal static class MemberInfoExtensions
{
    public static Type? GetReturnType(this MemberInfo member)
    {
        return member switch
        {
            MethodInfo method => method.ReturnType,
            PropertyInfo property => property.PropertyType,
            FieldInfo field => field.FieldType,
            EventInfo @event => @event.EventHandlerType,
            _ => null,
        };
    }

    public static bool IsReadOnly(this MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.IsInitOnly || field.IsLiteral,
            PropertyInfo property => property.SetMethod == null,
            _ => Throw(),
        };

        static bool Throw() => throw new ArgumentException("Member is not a field or property.", nameof(member));
    }
}

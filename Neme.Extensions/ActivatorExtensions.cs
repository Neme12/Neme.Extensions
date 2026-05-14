using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Neme.Extensions.Contracts;

namespace Neme.Extensions;

public static class ActivatorExtensions
{
    public static object? CreateDefaultValue(Type type)
    {
        Require.ArgumentNotNull(type);
        Require.ArgumentValid(type, !type.IsByRef);
        Require.ArgumentValid(type, !type.IsGenericType || type.IsConstructedGenericType);
        Require.ArgumentValid(type, !type.IsGenericParameter);

        if (type.GetGenericTypeDefinitionOrSelf() == typeof(Nullable<>))
            return null;

        if (type.IsValueType)
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return RuntimeHelpers.GetUninitializedObject(type);
#else
            return FormatterServices.GetUninitializedObject(type);
#endif
        }

        return null;
    }
}

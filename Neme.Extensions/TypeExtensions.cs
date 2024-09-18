using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Extensions;

internal static class TypeExtensions
{
    public static MethodInfo? GetMethod<TDelegate>(this Type @type, string name, BindingFlags bindingAttr)
        where TDelegate : Delegate
    {
        var invokeMethod = typeof(TDelegate).GetMethod("Invoke");
        if (invokeMethod is null)
            throw new ArgumentException("The delegate type is missing an Invoke method.", nameof(TDelegate));

        var genericParameterCount = GetGenericParameterCount(invokeMethod);

        var method = @type.GetMethod(
            name,
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            genericParameterCount: genericParameterCount,
#endif
            bindingAttr | BindingFlags.ExactBinding,
            binder: null,
            invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray(),
            modifiers: null);

        if (method is null)
            return null;

#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
		if (GetGenericParameterCount(method) != genericParameterCount)
			return null;
#endif

        if (method.ReturnType != invokeMethod.ReturnType)
            return null;

        return method;

        static int GetGenericParameterCount(MethodInfo method) =>
            method.ContainsGenericParameters ? method.GetGenericArguments().Length : 0;
    }
}

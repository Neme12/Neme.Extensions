using Neme.Extensions.Collections;
using System.Reflection;

namespace Neme.Extensions.Reflection;

public static class MethodInfoExtensions
{
    private static readonly EqualityComparer<ParameterInfo> _parameterEqualityComparer =
        EqualityComparerExtensions<ParameterInfo>.CreateBy(static p => (p.ParameterType, p.IsOut, p.IsIn));

    public static bool MatchesDelegate<TDelegate>(this MethodInfo methodInfo) where TDelegate : Delegate
    {
        if (methodInfo is null)
            throw new ArgumentNullException(nameof(methodInfo));

        var invokeMethod = typeof(TDelegate).GetInvokeMethod();

        if (!methodInfo.GetParameters().SequenceEqual(invokeMethod.GetParameters(), _parameterEqualityComparer))
            return false;

        if (!_parameterEqualityComparer.Equals(methodInfo.ReturnParameter, invokeMethod.ReturnParameter))
            return false;

        return true;
    }
}

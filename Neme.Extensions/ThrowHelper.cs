using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowNotSupported_ReadOnlyCollection() =>
        throw new NotSupportedException("Collection is read-only.");

    [DoesNotReturn]
    public static void ThrowArgument_FormatStringNotSupported(string paramName) =>
        throw new ArgumentException("Format string is not supported. The parameter must be null.", paramName);

    [DoesNotReturn]
    public static int ThrowArgument_ObjectMustBeOfType(string paramName, Type type) =>
        throw new ArgumentException($"Object must be of type {type}.", paramName);
}

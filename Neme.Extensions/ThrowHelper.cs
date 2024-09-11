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

    [DoesNotReturn]
    public static T ThrowFormat<T>(string input, Exception? innerException = null)
    {
        throw new FormatException($"The input string '{input}' was not in a correct format.", innerException);
    }

    [DoesNotReturn]
    public static T ThrowFormat<T>(ReadOnlySpan<char> input, Exception? innerException = null)
    {
#if NET6_0_OR_GREATER
        throw new FormatException($"The input string '{input}' was not in a correct format.", innerException);
#else
        throw new FormatException($"The input string '{input.ToString()}' was not in a correct format.");
#endif
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neme.Utilities.Contracts;

public static class Throw
{
    [DoesNotReturn]
    public static void InvalidOperationException()
    {
        throw new InvalidOperationException();
    }

    [DoesNotReturn]
    public static void ArgumentException<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        throw new ArgumentException(message, paramName: paramName);
    }

    [DoesNotReturn]
    public static void ArgumentException<T>(
        ReadOnlySpan<T> argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        throw new ArgumentException(message, paramName: paramName);
    }

    [DoesNotReturn]
    public static void ArgumentOutOfRangeException<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        throw new ArgumentOutOfRangeException(paramName, message);
    }
}

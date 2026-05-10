using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Contracts;

public static class Assert
{
    public static Exception Unreachable(string? message = null)
    {
        return new UnreachableException(message);
    }

    public static bool True([DoesNotReturnIf(false)] bool condition, string? message = null)
    {
        if (!condition)
            Fail(message);

        return condition;
    }

    public static bool False([DoesNotReturnIf(true)] bool condition, string? message = null)
    {
        if (!condition)
            Fail(message);

        return condition;
    }

    public static T? Null<T>([MaybeNull] T? value, string? message = null)
    {
        if (value is not null)
            Fail(message);

        return value;
    }

    public static T NotNull<T>([NotNull] this T? value, string? message = null)
    {
        if (value is null)
            Fail(message);

        return value;
    }

    public static T? Default<T>([MaybeNull] T? value, string? message = null)
    {
        if (!EqualityComparer<T?>.Default.Equals(value, default))
            Fail(message);

        return value;
    }

    public static T NotDefault<T>([NotNull] this T? value, string? message = null)
    {
        if (EqualityComparer<T?>.Default.Equals(value, default))
            Fail(message);

#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
        return value!;
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.
    }

    [DoesNotReturn]
    private static void Fail(string? message, [CallerMemberName] string? memberName = null)
    {
        Debug.Assert(memberName is not null);

        var failMessage = message is not null
            ? $"{nameof(Assert)}.{memberName} failed for value: {message}"
            : $"{nameof(Assert)}.{memberName} failed for value.";

        throw new AssertionFailedException(failMessage);
    }
}

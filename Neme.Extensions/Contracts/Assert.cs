using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Contracts;

public static class Assert
{
    public static void True([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition))] string? conditionText = null)
    {
        if (!condition)
            throw new AssertionFailedException($"Assertion failed: {conditionText}");
    }

    public static T NotNull<T>([NotNull] this T? value, [CallerArgumentExpression(nameof(value))] string? valueText = null)
    {
        if (value is null)
            throw new AssertionFailedException($"{nameof(Assert)}.{nameof(NotNull)} failed for value: {valueText}", value);

        return value;
    }

    public static T NotDefault<T>(this T value, [CallerArgumentExpression(nameof(value))] string? valueText = null) where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new AssertionFailedException($"{nameof(Assert)}.{nameof(NotDefault)} failed for value: {valueText}", value);

        return value;
    }
}

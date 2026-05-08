using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Contracts;

public static class Assert
{
    public static Exception Unreachable(string? message = null)
    {
        Trace.Fail(message);
        return new UnreachableException(message);
    }

    [Conditional("DEBUG")]
    public static void True([DoesNotReturnIf(false)] bool condition, string? message = null)
    {
        if (!condition)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void False([DoesNotReturnIf(true)] bool condition, string? message = null)
    {
        if (!condition)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void Null<T>([MaybeNull] T? value, string? message = null)
    {
        if (value is not null)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotNull<T>([NotNull] T? value, string? message = null)
    {
        if (value is null)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void Default<T>([MaybeNull] T? value, string? message = null)
    {
        if (!EqualityComparer<T?>.Default.Equals(value, default))
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotDefault<T>([NotNull] T? value, string? message = null)
    {
        if (EqualityComparer<T?>.Default.Equals(value, default))
            Fail(message);
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    [Conditional("DEBUG")]
    public static void NotEmpty<T>(ReadOnlySpan<T> value, string? message = null)
    {
        if (value.IsEmpty)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotEmpty<T>(IReadOnlyCollection<T>? value, string? message = null)
    {
        if (value is { Count: 0 })
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotEmpty<T, TCollection>(in TCollection? value, string? message = null)
        where TCollection : IReadOnlyCollection<T>
    {
        if (value is { Count: 0 })
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotNullOrEmpty<T>([NotNull] IReadOnlyCollection<T>? value, string? message = null)
    {
        if (value is null or { Count: 0 })
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotEmpty(string? value, string? message = null)
    {
        if (value is "")
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotNullOrEmpty([NotNull] string? value, string? message = null)
    {
        if (string.IsNullOrEmpty(value))
            Fail(message);
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    [Conditional("DEBUG")]
    public static void ItemsNotNull<T>(IReadOnlyList<T>? argument, string? message = null)
    {
        if (argument is not null)
        {
            foreach (var item in argument)
            {
                if (item is null)
                    Fail(message);
            }
        }
    }

    [Conditional("DEBUG")]
    public static void NotNullAndItemsNotNull<T>([NotNull] IReadOnlyList<T>? argument, string? message = null)
    {
        if (argument is null)
            Fail(message);

        foreach (var item in argument)
        {
            if (item is null)
                Fail(message);
        }
    }

    [Conditional("DEBUG")]
    public static void Zero<T>(T value, string? message = null)
        where T :
#if NET7_0_OR_GREATER
            INumberBase<T>
#else
            IEquatable<T>,
            new()
#endif
    {
        var isZero =
#if NET7_0_OR_GREATER
            T.IsZero(value);
#else
            EqualityComparer<T>.Default.Equals(value, new T());
#endif

        if (!isZero)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotZero<T>(T value, string? message = null)
        where T :
#if NET7_0_OR_GREATER
            INumberBase<T>
#else
            IEquatable<T>,
            new()
#endif
    {
        var isZero =
#if NET7_0_OR_GREATER
            T.IsZero(value);
#else
            EqualityComparer<T>.Default.Equals(value, new T());
#endif

        if (isZero)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void Positive<T>(T value, string? message = null)
        where T :
#if NET7_0_OR_GREATER
            INumberBase<T>
#else
            IComparable<T>,
            new()
#endif
    {
        var isPositive =
#if NET7_0_OR_GREATER
            T.IsPositive(value);
#else
            Comparer<T>.Default.Compare(value, new T()) > 0;
#endif

        if (!isPositive)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotPositive<T>(T value, string? message = null)
        where T :
#if NET7_0_OR_GREATER
            INumberBase<T>
#else
            IComparable<T>,
            new()
#endif
    {
        var isPositive =
#if NET7_0_OR_GREATER
            T.IsPositive(value);
#else
            Comparer<T>.Default.Compare(value, new T()) > 0;
#endif

        if (isPositive)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void Negative<T>(T value, string? message = null)
        where T :
#if NET7_0_OR_GREATER
            INumberBase<T>
#else
            IComparable<T>,
            new()
#endif
    {
        var isNegative =
#if NET7_0_OR_GREATER
            T.IsNegative(value);
#else
            Comparer<T>.Default.Compare(value, new T()) < 0;
#endif

        if (!isNegative)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotNegative<T>(T value, string? message = null)
        where T :
#if NET7_0_OR_GREATER
            INumberBase<T>
#else
            IComparable<T>,
            new()
#endif
    {
        var isNegative =
#if NET7_0_OR_GREATER
            T.IsNegative(value);
#else
            Comparer<T>.Default.Compare(value, new T()) < 0;
#endif

        if (isNegative)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void Equal<T>(T value, T otherValue, string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(value, otherValue))
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void NotEqual<T>(T value, T otherValue, string? message = null)
    {
        if (EqualityComparer<T>.Default.Equals(value, otherValue))
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void GreaterThan<T>(T value, T otherValue, string? message = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(value, otherValue) <= 0)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void GreaterThanOrEqual<T>(T value, T otherValue, string? message = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(value, otherValue) < 0)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void LessThan<T>(T value, T otherValue, string? message = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(value, otherValue) >= 0)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void LessThanOrEqual<T>(T value, T otherValue, string? message = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(value, otherValue) > 0)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void InRange<T>(
       T value,
       T lowerInclusive,
       T upperInclusive,
       string? message = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(lowerInclusive) < 0 || value.CompareTo(upperInclusive) > 0)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void TypeEqual<T>(
        T argument,
        Type type,
        string? message = null)
    {
        if (argument is not null && argument.GetType() != type)
            Fail(message);
    }

    [Conditional("DEBUG")]
    public static void Defined<T>(
        T argument,
        string? message = null)
        where T : struct, Enum
    {
        if (!EnumPolyfill.IsDefined(argument))
            Fail(message);
    }

    [DoesNotReturn]
    private static void Fail(string? message, [CallerMemberName] string? memberName = null)
    {
        Debug.Assert(memberName is not null);

        var failMessage = message is not null
            ? $"{nameof(Assert)}.{memberName} failed: {message}"
            : $"{nameof(Assert)}.{memberName} failed.";

        Debug.Fail(failMessage);
#pragma warning disable CS8763 // A method marked [DoesNotReturn] should not return.
    }
#pragma warning restore CS8763 // A method marked [DoesNotReturn] should not return.
}

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Contracts;

#pragma warning disable CA1062 // Validate arguments of public methods

public static class Require
{
    public static void True([DoesNotReturnIf(false)] bool condition, string? message = null)
    {
        if (!condition)
            ThrowInvalidOperationException(message);
    }

    public static void NotDisposed([DoesNotReturnIf(true)] bool disposed, object instance)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(disposed, instance);
#else
        ThrowObjectDisposedException(instance, null);
#endif
    }

    public static void NotDisposed([DoesNotReturnIf(true)] bool disposed, Type type)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(disposed, type);
#else
        if (disposed)
            ThrowObjectDisposedException(type, null);
#endif
    }

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
            throw new InvalidOperationException();
    }
    
    public static void ArgumentTrue(
        bool argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (!argument)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentFalse(
        bool argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNull<T>(
        [MaybeNull] T? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is not null)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotNull<T>(
        [NotNull] T? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
            ThrowArgumentNullException(paramName, message);
    }

    public static void ArgumentDefault<T>(
        [MaybeNull] T? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (!EqualityComparer<T?>.Default.Equals(argument, default))
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotDefault<T>(
        [NotNull] T? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (EqualityComparer<T?>.Default.Equals(argument, default))
            ThrowArgumentException(paramName, argument, message);
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    public static void ArgumentNotEmpty<T>(
        ReadOnlySpan<T> argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.IsEmpty)
            ThrowArgumentException(paramName, argument.ToString(), message);
    }

    public static void ArgumentNotEmpty<T>(
        IReadOnlyCollection<T>? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is { Count: 0})
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotNullOrEmpty<T>(
        [NotNull] IReadOnlyCollection<T>? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null or { Count: 0})
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotEmpty<T>(
        string? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is "")
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNullOrEmpty(
        string? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (!string.IsNullOrEmpty(argument))
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotNullOrEmpty(
        [NotNull] string? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
            Throw(argument, paramName, message);

        [DoesNotReturn]
        static void Throw(string? argument, string? paramName, string? message)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName, message);
            else
                throw new ArgumentException(paramName, message);
        }
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.
    }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

    public static void ArgumentItemsNotNull<T>(
        IReadOnlyList<T>? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is not null)
        {
            foreach (var item in argument)
            {
                if (item is null)
                    ThrowArgumentException(paramName, argument, message);
            }
        }
    }

    public static void ArgumentNotNullAndItemsNotNull<T>(
        [NotNull] IReadOnlyList<T>? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
            ThrowArgumentNullException(paramName, message);

        foreach (var item in argument)
        {
            if (item is null)
                ThrowArgumentException(paramName, argument, message);
        }
    }

    public static void ArgumentZero<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
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
            T.IsZero(argument);
#else
            EqualityComparer<T>.Default.Equals(argument, new T());
#endif

        if (!isZero)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotZero<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
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
            T.IsZero(argument);
#else
            EqualityComparer<T>.Default.Equals(argument, new T());
#endif

        if (isZero)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentPositive<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
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
            T.IsPositive(argument);
#else
            Comparer<T>.Default.Compare(argument, new T()) > 0;
#endif

        if (!isPositive)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotPositive<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
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
            T.IsPositive(argument);
#else
            Comparer<T>.Default.Compare(argument, new T()) > 0;
#endif

        if (isPositive)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNegative<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
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
            T.IsNegative(argument);
#else
            Comparer<T>.Default.Compare(argument, new T()) < 0;
#endif

        if (!isNegative)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotNegative<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
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
            T.IsNegative(argument);
#else
            Comparer<T>.Default.Compare(argument, new T()) < 0;
#endif

        if (isNegative)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentEqual<T>(
        T argument,
        T value,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(argument, value))
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentNotEqual<T>(
        T argument,
        T value,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (EqualityComparer<T>.Default.Equals(argument, value))
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentGreaterThan<T>(
        T argument,
        T value,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(argument, value) <= 0)
            ThrowArgumentOutOfRangeException(paramName, message);
    }

    public static void ArgumentGreaterThanOrEqual<T>(
        T argument,
        T value,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(argument, value) < 0)
            ThrowArgumentOutOfRangeException(paramName, message);
    }

    public static void ArgumentLessThan<T>(
        T argument,
        T value,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(argument, value) >= 0)
            ThrowArgumentOutOfRangeException(paramName, message);
    }

    public static void ArgumentLessThanOrEqual<T>(
        T argument,
        T value,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : IComparable<T>
    {
        if (Comparer<T>.Default.Compare(argument, value) > 0)
            ThrowArgumentOutOfRangeException(paramName, message);
    }

    public static void ArgumentInRange<T>(
        T argument,
        T lowerInclusive,
        T upperInclusive,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : IComparable<T>
    {
        if (argument.CompareTo(lowerInclusive) < 0 || argument.CompareTo(upperInclusive) > 0)
            ThrowArgumentOutOfRangeException(paramName, message);
    }

    public static void ArgumentTypeEqual<T>(
        T argument,
        Type type,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is not null && argument.GetType() != type)
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentDefined<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct, Enum
    {
        if (!EnumPolyfill.IsDefined(argument))
            ThrowArgumentException(paramName, argument, message);
    }

    public static void ArgumentFlagsDefined<T>(
        T argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where T : struct, Enum
    {
        // TODO: Implement this method.
        throw new NotImplementedException();
        //if (!typeof(T).IsDefined(typeof(FlagsAttribute), inherit: false))
        //    Throw.ArgumentException(default(object?), paramName: nameof(T));

        //var value = Enum.get default(T);

        //foreach (var flag in allFlags)
        //{
        //    value |= flag;
        //}
    }

    public static void ArgumentValid<T>(
        T argument,
        bool isValid,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null,
        [CallerArgumentExpression(nameof(isValid))] string? condition = null)
    {
        if (!isValid)
            ThrowArgumentInvalidException(paramName, argument, condition);
    }

    public static void ArgumentValid<T>(
        ReadOnlySpan<T> argument,
        bool isValid,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null,
        [CallerArgumentExpression(nameof(isValid))] string? condition = null)
    {
        if (!isValid)
            ThrowArgumentInvalidException(paramName, argument.ToString(), condition);
    }

    public static void ArgumentValid<T>(
        Span<T> argument,
        bool isValid,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null,
        [CallerArgumentExpression(nameof(isValid))] string? condition = null)
    {
        if (!isValid)
            ThrowArgumentInvalidException(paramName, argument.ToString(), condition);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentInvalidException(string? paramName, object? actualValue, string? condition) =>
        throw new ArgumentInvalidException(paramName, actualValue, condition);

    [DoesNotReturn]
    private static void ThrowArgumentException(string? paramName, object? actualValue, string? message) =>
        throw new ArgumentException2(paramName, actualValue, message);

    [DoesNotReturn]
    private static void ThrowArgumentNullException(string? paramName, string? message) =>
        throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    private static void ThrowArgumentOutOfRangeException(string? paramName, string? message) =>
        throw new ArgumentOutOfRangeException(paramName);

    [DoesNotReturn]
    private static void ThrowInvalidOperationException(string? message) =>
        throw new InvalidOperationException();

    [DoesNotReturn]
    private static void ThrowObjectDisposedException(object instance, string? message) =>
        throw new ObjectDisposedException(instance.GetType().FullName);

    [DoesNotReturn]
    private static void ThrowObjectDisposedException(Type type, string? message) =>
        throw new ObjectDisposedException(type.FullName);
}

namespace Neme.Extensions;

public static class Optional
{
    public static Optional<T> Some<T>(T value) =>
        new(value);

    public static Optional<T> FromNullable<T>(T? value) where T : struct =>
        value ?? Optional<T>.None;

    public static Optional<T> FromNullable<T>(T? value) where T : class =>
        value ?? Optional<T>.None;

    public static ref readonly bool GetHasValueRef<T>(in Optional<T> optional) =>
        ref optional._hasValue;

    public static ref readonly T? GetValueRefOrDefaultRef<T>(in Optional<T> optional) =>
        ref optional._value;

    public static bool Equals<T>(Optional<T> x, Optional<T> y) =>
        x.Equals(y);

    public static bool Equals<T>(Optional<T> x, Optional<T> y, IEqualityComparer<T>? valueComparer) =>
        x.Equals(y, valueComparer);

    public static int Compare<T>(Optional<T> x, Optional<T> y) =>
        x.CompareTo(y);

    public static int Compare<T>(Optional<T> x, Optional<T> y, IComparer<T>? valueComparer) =>
        x.CompareTo(y, valueComparer);

    public static Type? GetUnderlyingType(Type optionalType)
    {
        if (optionalType is null)
            throw new ArgumentNullException(nameof(optionalType));

        if (optionalType.IsConstructedGenericType && optionalType.GetGenericTypeDefinition() == typeof(Optional<>))
            return optionalType.GetGenericArguments()[0];

        return null;
    }
}

public static class OptionalValueTypeExtensions
{
    public static T? AsNullable<T>(this Optional<T> optional)
        where T : struct
    {
        return optional.TryGetValue(out var value) ? value : null;
    }
}

public static class OptionalReferenceTypeExtensions
{
    public static T? AsNullable<T>(this Optional<T> optional)
        where T : class
    {
        return optional.TryGetValue(out var value) ? value : null;
    }
}

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
        EqualityComparer<Optional<T>>.Default.Equals(x, y);

    public static int Compare<T>(Optional<T> x, Optional<T> y) =>
        Comparer<Optional<T>>.Default.Compare(x, y);
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

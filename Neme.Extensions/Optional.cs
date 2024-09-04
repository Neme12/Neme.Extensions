namespace Neme.Extensions;

public static class Optional
{
    public static Optional<T> Some<T>(T value) =>
        new(value);

    public static Optional<T> FromNullable<T>(T? value) where T : struct =>
        value ?? Optional<T>.None;

    public static Optional<T> FromNullable<T>(T? value) where T : class =>
        value ?? Optional<T>.None;
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

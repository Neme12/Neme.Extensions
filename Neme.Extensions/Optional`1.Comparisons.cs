using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

public readonly partial struct Optional<T>
{
	public bool Equals(Optional<T> other) =>
        Equals(other, EqualityComparer<T>.Default);

    public bool Equals(Optional<T> other, IEqualityComparer<T> elementComparer)
	{
        if (elementComparer is null)
            throw new ArgumentNullException(nameof(elementComparer));

        return (_hasValue, other._hasValue) switch
        {
            (true, true) => elementComparer.Equals(_value!, other._value!),
            (false, false) => true,
            _ => false,
        };
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Optional<T> other && Equals(other);

    public override int GetHashCode() =>
        GetHashCode(EqualityComparer<T>.Default);

    public int GetHashCode(IEqualityComparer<T> elementComparer)
    {
        if (elementComparer is null)
            throw new ArgumentNullException(nameof(elementComparer));

        if (_hasValue)
            return elementComparer.GetHashCode(_value!);

        return -1;
    }
}

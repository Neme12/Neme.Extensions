using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

public readonly partial struct Optional<T>
{
	public bool Equals(Optional<T> other)
	{
        return (_hasValue, other._hasValue) switch
        {
            (true, true) => EqualityComparer<T>.Default.Equals(_value!, other._value!),
            (false, false) => true,
            _ => false,
        };
    }

    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Optional<T> other && Equals(other);
}

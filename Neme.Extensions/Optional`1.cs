using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions;

[StructLayout(LayoutKind.Auto)]
public readonly partial struct Optional<T> :
    IEquatable<Optional<T>>,
    IComparable<Optional<T>>,
    IComparable,
    IStructuralEquatable,
    IStructuralComparable
#if NETCOREAPP2_0_OR_GREATER || NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    , ITuple
#endif
#if NET7_0_OR_GREATER
    , IEqualityOperators<Optional<T>, Optional<T>, bool>
#endif
{
    private readonly bool _hasValue;
    private readonly T? _value;

    public Optional(T value)
    {
        _hasValue = true;
        _value = value;
    }

    public bool HasValue =>
        _hasValue;

    public T Value
    {
        get
        {
            if (!_hasValue)
                throw new InvalidOperationException("Optional has no value.");

            return _value!;
        }
    }

    public T? GetValueOrDefault() =>
        _value;

    public T? GetValueOrDefault(T? defaultValue) =>
        _hasValue ? _value : defaultValue;

    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _hasValue;
    }

    public void Deconstruct(out bool hasValue, out T? value)
    {
        hasValue = _hasValue;
        value = _value;
    }

#if NETCOREAPP2_0_OR_GREATER || NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    int ITuple.Length =>
        _hasValue ? 1 : 0;

    object? ITuple.this[int index] =>
        _hasValue && index == 0 ? _value : throw new ArgumentOutOfRangeException(nameof(index));
#endif

    public static implicit operator Optional<T>(T value) =>
        new(value);

    public static explicit operator T(Optional<T> optional) =>
        optional.Value;

    public static Optional<T> None => default;
}

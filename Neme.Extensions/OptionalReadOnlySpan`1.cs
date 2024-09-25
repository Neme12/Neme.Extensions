using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Extensions;

internal readonly ref struct OptionalReadOnlySpan<T>
{
    internal readonly bool _hasValue;
    internal readonly ReadOnlySpan<T> _value;

    public OptionalReadOnlySpan(ReadOnlySpan<T> value)
    {
        _hasValue = true;
        _value = value;
    }

    public bool HasValue =>
        _hasValue;

    public ReadOnlySpan<T> Value
    {
        get
        {
            if (!_hasValue)
                Throw();

            return _value!;

            [DoesNotReturn]
            static void Throw() =>
                throw new InvalidOperationException("Optional has no value.");
        }
    }

    public ReadOnlySpan<T> GetValueOrDefault() =>
        _value;

    public ReadOnlySpan<T> GetValueOrDefault(ReadOnlySpan<T> defaultValue) =>
        _hasValue ? _value : defaultValue;

    public bool TryGetValue(out ReadOnlySpan<T> value)
    {
        value = _value;
        return _hasValue;
    }

    public void Deconstruct(out bool hasValue, out ReadOnlySpan<T> value)
    {
        hasValue = _hasValue;
        value = _value;
    }

    public static implicit operator OptionalReadOnlySpan<T>(ReadOnlySpan<T> value) =>
        new(value);

    public static explicit operator ReadOnlySpan<T>(OptionalReadOnlySpan<T> optional) =>
        optional.Value;

    public static OptionalReadOnlySpan<T> None => default;
}

using Neme.Extensions.Utilities;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Neme.Extensions;

[Serializable]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct Optional<T> :
    IEquatable<Optional<T>>,
    IComparable<Optional<T>>,
    IComparable,
    IStructuralEquatable,
    IStructuralComparable,
    IFormattable,
    ISerializable
#if NETCOREAPP2_0_OR_GREATER || NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    , ITuple
#endif
#if NET6_0_OR_GREATER
    , ISpanFormattable
#endif
#if NET7_0_OR_GREATER
    , IEqualityOperators<Optional<T>, Optional<T>, bool>
    , IParsable<Optional<T>>
    , ISpanParsable<Optional<T>>
#endif
{
    internal readonly bool _hasValue;
    internal readonly T? _value;

    public Optional(T value)
    {
        _hasValue = true;
        _value = value;
    }

    private Optional(SerializationInfo info, StreamingContext context)
    {
        if (info is null)
            throw new ArgumentNullException(nameof(info));

        if (info.MemberCount != 0)
        {
            _hasValue = true;
            _value = (T)info.GetValue("Value", typeof(T))!;
        }
    }

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (_hasValue)
            info.AddValue("Value", _value);
    }

    public bool HasValue =>
        _hasValue;

    public T Value
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

    public override string ToString() =>
        ToString(format: null, formatProvider: null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (format is not (null or ""))
            ThrowHelper.ThrowArgument_FormatStringNotSupported(nameof(format));

        return (_hasValue, _value) switch
        {
            (true, null) => "Some { }",
            (true, var value) =>
#if NET6_0_OR_GREATER
                string.Create(formatProvider, $"Some {{ {value} }}"),
#else
                ((FormattableString)$"Some {{ {value} }}").ToString(formatProvider),
#endif
            _ => "None",
        };
    }

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (format.Length > 0)
            ThrowHelper.ThrowArgument_FormatStringNotSupported(nameof(format));

        return (_hasValue, _value) switch
        {
            (true, null) => destination.TryWrite("Some { }", out charsWritten),
            (true, var value) => destination.TryWrite(provider, $"Some {{ {value} }}", out charsWritten),
            _ => destination.TryWrite("None", out charsWritten),
        };
    }
#endif

    private static ValueLazy<ParseProviderDelegate?> s_parseMethodLazy;
    private static ValueLazy<ParseSpanProviderDelegate?> s_parseSpanMethodLazy;
    private static ValueLazy<TryParseProviderDelegate?> s_tryParseMethodLazy;
    private static ValueLazy<TryParseSpanProviderDelegate?> s_tryParseSpanMethodLazy;

    private delegate T ParseDelegate(string s);
    private delegate T ParseProviderDelegate(string s, IFormatProvider? provider);
    private delegate T ParseNumberStylesProviderDelegate(string s, NumberStyles style, IFormatProvider? provider);
    private delegate T ParseSpanDelegate(ReadOnlySpan<char> s);
    private delegate T ParseSpanProviderDelegate(ReadOnlySpan<char> s, IFormatProvider? provider);
    private delegate T ParseSpanNumberStylesProviderDelegate(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider);
    private delegate bool TryParseDelegate([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out T result);
    private delegate bool TryParseProviderDelegate([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
    private delegate bool TryParseNumberStylesProviderDelegate([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
    private delegate bool TryParseSpanDelegate(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out T result);
    private delegate bool TryParseSpanProviderDelegate(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
    private delegate bool TryParseSpanNumberStylesProviderDelegate(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);

    public static Optional<T> Parse(string s) =>
        Parse(s, provider: null);

    public static Optional<T> Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
            throw new ArgumentNullException(nameof(s));

        switch (ParseCore(s.AsSpan(), provider, out var hasInside, out var inside))
        {
            case ParseResult.None:
                return None;
            case ParseResult.Some:
                if (!hasInside)
                    return new(default!);

                if (typeof(T) == typeof(string))
                    return new((T)(object)inside.ToString());

                if (GetParseSpanMethod() is { } spanMethod)
                {
                    try
                    {
                        return new(spanMethod.Invoke(inside, provider));
                    }
                    catch (FormatException e)
                    {
                        ThrowHelper.ThrowFormat<Optional<T>>(s, e);
                    }
                }

                if (GetParseMethod() is { } stringMethod)
                {
                    try
                    {
                        return new(stringMethod.Invoke(inside.ToString(), provider));
                    }
                    catch (FormatException e)
                    {
                        ThrowHelper.ThrowFormat<Optional<T>>(s, e);
                    }
                }

                ThrowNoParseMethod(nameof(Parse));
                break;
        }

        return ThrowHelper.ThrowFormat<Optional<T>>(s);
    }

    public static Optional<T> Parse(ReadOnlySpan<char> s) =>
        Parse(s, provider: null);

    public static Optional<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        switch (ParseCore(s, provider, out var hasInside, out var inside))
        {
            case ParseResult.None:
                return None;
            case ParseResult.Some:
                if (!hasInside)
                    return new(default!);

                if (typeof(T) == typeof(string))
                    return new((T)(object)inside.ToString());

                if (GetParseSpanMethod() is { } spanMethod)
                {
                    try
                    {
                        return new(spanMethod.Invoke(inside, provider));
                    }
                    catch (FormatException e)
                    {
                        ThrowHelper.ThrowFormat<Optional<T>>(s, e);
                    }
                }

                ThrowNoParseMethod(nameof(Parse));
                break;
        }

        return ThrowHelper.ThrowFormat<Optional<T>>(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out Optional<T> result) =>
        TryParse(s, provider: null, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Optional<T> result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        switch (ParseCore(s.AsSpan(), provider, out var hasInside, out var inside))
        {
            case ParseResult.None:
                result = None;
                return true;
            case ParseResult.Some:
                if (!hasInside)
                {
                    result = new(default!);
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    result = new((T)(object)inside.ToString());
                    return true;
                }

                if (GetTryParseSpanMethod() is { } spanMethod)
                {
                    if (spanMethod.Invoke(inside, provider, out var value))
                    {
                        result = new(value);
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }

                if (GetTryParseMethod() is { } stringMethod)
                {
                    if (stringMethod.Invoke(inside.ToString(), provider, out var value))
                    {
                        result = new(value);
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }

                ThrowNoParseMethod(nameof(TryParse));
                break;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, out Optional<T> result) =>
        TryParse(s, provider: null, out result);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Optional<T> result)
    {
        switch (ParseCore(s, provider, out var hasInside, out var inside))
        {
            case ParseResult.None:
                result = None;
                return true;
            case ParseResult.Some:
                if (!hasInside)
                {
                    result = new(default!);
                    return true;
                }

                if (typeof(T) == typeof(string))
                {
                    result = new((T)(object)inside.ToString());
                    return true;
                }

                if (GetTryParseSpanMethod() is { } spanMethod)
                {
                    if (spanMethod.Invoke(inside, provider, out var value))
                    {
                        result = new(value);
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }

                ThrowNoParseMethod(nameof(TryParse));
                break;
        }

        result = default;
        return false;
    }

    private static ParseProviderDelegate? GetParseMethod()
    {
        return s_parseMethodLazy.EnsureInitialized(
            static () =>
            {
                if (GetParseMethod<ParseProviderDelegate>(nameof(Parse)) is { } method)
                    return method;

                if (GetParseMethod<ParseNumberStylesProviderDelegate>(nameof(Parse)) is { } numberStylesMethod)
                    return (s, provider) => numberStylesMethod(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);

                if (GetParseMethod<ParseDelegate>(nameof(Parse)) is { } noProviderMethod)
                    return (s, provider) => noProviderMethod(s);

                return null;
            });
    }

    private static ParseSpanProviderDelegate? GetParseSpanMethod()
    {
        return s_parseSpanMethodLazy.EnsureInitialized(
            static () =>
            {
                if (GetParseMethod<ParseSpanProviderDelegate>(nameof(Parse)) is { } method)
                    return method;

                if (GetParseMethod<ParseSpanNumberStylesProviderDelegate>(nameof(Parse)) is { } numberStylesMethod)
                    return (s, provider) => numberStylesMethod(s, NumberStyles.Float | NumberStyles.AllowThousands, provider);

                if (GetParseMethod<ParseSpanDelegate>(nameof(Parse)) is { } noProviderMethod)
                    return (s, provider) => noProviderMethod(s);

                return null;
            });

    }

    private static TryParseProviderDelegate? GetTryParseMethod()
    {
        return s_tryParseMethodLazy.EnsureInitialized(
            static () =>
            {
                if (GetParseMethod<TryParseProviderDelegate>(nameof(TryParse)) is { } method)
                    return method;

                if (GetParseMethod<TryParseNumberStylesProviderDelegate>(nameof(TryParse)) is { } numberStylesMethod)
                    return ([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => numberStylesMethod(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

                if (GetParseMethod<TryParseDelegate>(nameof(TryParse)) is { } noProviderMethod)
                    return ([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => noProviderMethod(s, out result);

                return null;
            });
    }

    private static TryParseSpanProviderDelegate? GetTryParseSpanMethod()
    {
        return s_tryParseSpanMethodLazy.EnsureInitialized(
            static () =>
            {
                if (GetParseMethod<TryParseSpanProviderDelegate>(nameof(TryParse)) is { } method)
                    return method;

                if (GetParseMethod<TryParseSpanNumberStylesProviderDelegate>(nameof(TryParse)) is { } numberStylesMethod)
                    return (ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => numberStylesMethod(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

                if (GetParseMethod<TryParseSpanDelegate>(nameof(TryParse)) is { } noProviderMethod)
                    return (ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => noProviderMethod(s, out result);

                return null;
            });
    }

    [DoesNotReturn]
    private static void ThrowNoParseMethod(string methodName) =>
        throw new NotSupportedException($"Type {typeof(T)} has no appropriate {methodName} method.");

    private enum ParseResult
    {
        Error,
        None,
        Some,
    }

    private static ParseResult ParseCore(ReadOnlySpan<char> s, IFormatProvider? provider, out bool hasInside, out ReadOnlySpan<char> inside)
    {
        if (s.Equals("None".AsSpan(), StringComparison.Ordinal))
        {
            hasInside = false;
            inside = default;
            return ParseResult.None;
        }

        const string prefix = "Some { ";
        const string suffix = " }";

        if (s.StartsWith(prefix.AsSpan(), StringComparison.Ordinal) && s.EndsWith(suffix.AsSpan(), StringComparison.Ordinal))
        {
            if (s.Length == prefix.Length + suffix.Length - 1)
            {
                hasInside = false;
                inside = default;
                return ParseResult.Some;
            }

            hasInside = true;
            inside = s[prefix.Length..^suffix.Length];
            return ParseResult.Some;
        }

        hasInside = false;
        inside = default;
        return ParseResult.Error;
    }

    private static TDelegate? GetParseMethod<TDelegate>(string methodName)
        where TDelegate : Delegate
    {
        Debug.Assert(methodName is "Parse" or "TryParse");

        var invokeMethod = typeof(TDelegate).GetMethod("Invoke")!;

        var method = typeof(T).GetMethod(
            methodName,
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            genericParameterCount: 0,
#endif
            BindingFlags.Public | BindingFlags.Static | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly,
            binder: null,
            invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray(),
            modifiers: null);

        if (method is null)
            return null;

#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
        if (method.ContainsGenericParameters)
            return null;
#endif

        if (method.ReturnType != invokeMethod.ReturnType)
            return null;

#if NET5_0_OR_GREATER
        return method.CreateDelegate<TDelegate>();
#else
        return (TDelegate)method.CreateDelegate(typeof(TDelegate));
#endif
    }

    public static implicit operator Optional<T>(T value) =>
        new(value);

    public static explicit operator T(Optional<T> optional) =>
        optional.Value;

    public static Optional<T> None => default;
}

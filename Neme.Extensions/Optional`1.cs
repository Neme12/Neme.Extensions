using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    private static ParseDelegate? s_parseMethod;
    private static bool s_parseMethodInitialized;
    private static object? s_parseMethodLock;

    private static TryParseDelegate? s_tryParseMethod;
    private static bool s_tryParseMethodInitialized;
    private static object? s_tryParseMethodLock;

    private delegate T ParseDelegate(string s, IFormatProvider? provider);
    private delegate bool TryParseDelegate([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);

    public static Optional<T> Parse(string s) =>
        Parse(s, provider: null);

    public static Optional<T> Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
            throw new ArgumentNullException(nameof(s));

        if (s == "None")
            return None;

        const string prefix = "Some { ";
        const string suffix = " }";

        if (s.StartsWith(prefix, StringComparison.Ordinal) && s.EndsWith(suffix, StringComparison.Ordinal))
        {
            if (s.Length == prefix.Length + suffix.Length - 1)
                return default(T)!;

            var inside = s[prefix.Length..^suffix.Length];

            if (typeof(T) == typeof(string))
                return (T)(object)inside;

            var method = LazyInitializer.EnsureInitialized(
                ref s_parseMethod,
                ref s_parseMethodInitialized,
                ref s_parseMethodLock,
                static () => GetParseMethod<ParseDelegate>("Parse"));

            Debug.Assert(s_parseMethodInitialized);

            if (method is null)
                throw new InvalidOperationException($"Type {typeof(T)} has no appropriate Parse method.");

            try
            {
                return new(method.Invoke(inside, provider));
            }
            catch (FormatException e)
            {
                throw new FormatException(null, e);
            }
        }

        throw new FormatException();
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

        if (s == "None")
        {
            result = None;
            return true;
        }

        const string prefix = "Some { ";
        const string suffix = " }";

        if (s.StartsWith(prefix, StringComparison.Ordinal) && s.EndsWith(suffix, StringComparison.Ordinal))
        {
            if (s.Length == prefix.Length + suffix.Length - 1)
            {
                result = default(T)!;
                return true;
            }

            var inside = s[prefix.Length..^suffix.Length];

            if (typeof(T) == typeof(string))
            {
                result = (T)(object)inside;
                return true;
            }

            var method = LazyInitializer.EnsureInitialized(
                ref s_tryParseMethod,
                ref s_tryParseMethodInitialized,
                ref s_tryParseMethodLock,
                static () => GetParseMethod<TryParseDelegate>("TryParse"));

            Debug.Assert(s_tryParseMethodInitialized);

            if (method is null)
                throw new InvalidOperationException($"Type {typeof(T)} has no appropriate TryParse method.");

            if (method.Invoke(inside, provider, out var value))
            {
                result = new(value);
                return true;
            }
        }

        result = default;
        return false;
    }

    private static TDelegate? GetParseMethod<TDelegate>(string methodName)
        where TDelegate : Delegate
    {
        Debug.Assert(methodName is "Parse" or "TryParse");

        var method = typeof(T).GetMethod(
            methodName,
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            genericParameterCount: 0,
#endif
            BindingFlags.Public | BindingFlags.Static | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly,
            binder: null,
            typeof(TDelegate).GetMethod("Invoke")!.GetParameters().Select(p => p.ParameterType).ToArray(),
            modifiers: null);

        if (method is null)
            return null;

#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
        if (method.ContainsGenericParameters)
            return null;
#endif

        if (method.ReturnType != typeof(TDelegate).GetMethod("Invoke")!.ReturnType)
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

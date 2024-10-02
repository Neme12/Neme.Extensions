using Neme.Extensions.CompilerServices;
using Neme.Extensions.Reflection;
using Neme.Extensions.Text;
using Neme.Extensions.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Neme.Extensions;

internal static class ParseHelper<T>
{
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

    public static T Parse(ReadOnlySpan<char> s, IFormatProvider? provider, bool allowStringMethod)
    {
        if (TryParseString(s, out var stringValue))
            return stringValue;

        if (GetParseSpanMethod() is { } spanMethod)
            return spanMethod.Invoke(s, provider);

        if (allowStringMethod && GetParseMethod() is { } stringMethod)
            return stringMethod.Invoke(s.ToString(), provider);

        ThrowNoParseMethod(nameof(Parse));
        return default;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, bool allowStringMethod, [MaybeNullWhen(false)] out T result)
    {
        if (TryParseString(s, out var stringValue))
        {
            result = stringValue;
            return true;
        }

        if (GetTryParseSpanMethod() is { } spanMethod)
            return spanMethod.Invoke(s, provider, out result);

        if (allowStringMethod && GetTryParseMethod() is { } stringMethod)
            return stringMethod.Invoke(s.ToString(), provider, out result);

        ThrowNoParseMethod(nameof(TryParse));
        result = default;
        return default;
    }

    [DoesNotReturn]
    private static void ThrowNoParseMethod(string methodName) =>
        throw new NotSupportedException($"Type {typeof(T)} has no appropriate {methodName} method.");

    private static bool TryParseString(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out T result)
    {
        if (typeof(T) == typeof(string))
        {
            result = (T)(object)s.ToString();
            return true;
        }

        if (typeof(T) == typeof(ReadOnlyMemory<char>))
        {
            result = (T)(object)s.ToString().AsMemory();
            return true;
        }

        if (typeof(T) == typeof(Memory<char>))
        {
            result = (T)(object)s.ToArray().AsMemory();
            return true;
        }

        result = default;
        return false;
    }

    private static ParseProviderDelegate? GetParseMethod()
    {
        return s_parseMethodLazy.EnsureInitialized(
            static () =>
            {
                if (typeof(T) == typeof(char))
                    return static (s, provider) => UnsafeExtensions.InAs<char, T>(char.Parse(s));

#if NETCOREAPP3_0_OR_GREATER
				if (typeof(T) == typeof(Rune))
					return static (s, provider) => UnsafeExtensions.InAs<Rune, T>(RuneExtensions.Parse(s));
#endif

#if NET7_0_OR_GREATER
                if (typeof(T).GetInterfaces().FirstOrDefault(i =>
                    i.IsConstructedGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IParsable<>) &&
                    i.GetGenericArguments()[0] == typeof(T)) is { } @interface)
                {
                    return typeof(T)
                        .GetInterfaceMap(@interface)
                        .GetImplementationMethod(@interface.GetMethod<ParseProviderDelegate>(nameof(int.Parse), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)!)
                        .CreateDelegate<ParseProviderDelegate>();
                }
#endif

                if (GetParseMethod<ParseProviderDelegate>(nameof(int.Parse)) is { } method)
                    return method;

                if (GetParseMethod<ParseNumberStylesProviderDelegate>(nameof(int.Parse), out var defaultStyle) is { } numberStylesMethod)
                {
                    var style = defaultStyle ?? GetDefaultNumberStyles();
                    return (s, provider) => numberStylesMethod(s, style, provider);
                }

                if (GetParseMethod<ParseDelegate>(nameof(int.Parse)) is { } noProviderMethod)
                    return (s, provider) => noProviderMethod(s);

                return null;
            });
    }

    private static ParseSpanProviderDelegate? GetParseSpanMethod()
    {
        return s_parseSpanMethodLazy.EnsureInitialized(
            static () =>
            {
                if (typeof(T) == typeof(char))
                    return static (s, provider) => UnsafeExtensions.InAs<char, T>(CharExtensions.Parse(s));

#if NETCOREAPP3_0_OR_GREATER
                if (typeof(T) == typeof(Rune))
                    return static (s, provider) => UnsafeExtensions.InAs<Rune, T>(RuneExtensions.Parse(s));
#endif

#if NET7_0_OR_GREATER
                if (typeof(T).GetInterfaces().FirstOrDefault(i =>
                    i.IsConstructedGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ISpanParsable<>) &&
                    i.GetGenericArguments()[0] == typeof(T)) is { } @interface)
                {
                    return typeof(T)
                        .GetInterfaceMap(@interface)
                        .GetImplementationMethod(@interface.GetMethod<ParseSpanProviderDelegate>(nameof(int.Parse), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)!)
                        .CreateDelegate<ParseSpanProviderDelegate>();
                }
#endif

                if (GetParseMethod<ParseSpanProviderDelegate>(nameof(int.Parse)) is { } method)
                    return method;

                if (GetParseMethod<ParseSpanNumberStylesProviderDelegate>(nameof(int.Parse), out var defaultStyle) is { } numberStylesMethod)
                {
                    var style = defaultStyle ?? GetDefaultNumberStyles();
                    return (s, provider) => numberStylesMethod(s, style, provider);
                }

                if (GetParseMethod<ParseSpanDelegate>(nameof(int.Parse)) is { } noProviderMethod)
                    return (s, provider) => noProviderMethod(s);

                return null;
            });

    }

    private static TryParseProviderDelegate? GetTryParseMethod()
    {
        return s_tryParseMethodLazy.EnsureInitialized(
            static () =>
            {
                if (typeof(T) == typeof(char))
                {
                    return static ([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) =>
                        char.TryParse(s, out UnsafeExtensions.OutAs<T, char>(out result));
                }

#if NETCOREAPP3_0_OR_GREATER
				if (typeof(T) == typeof(Rune))
				{
					return static ([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) =>
						RuneExtensions.TryParse(s, out UnsafeExtensions.OutAs<T, Rune>(out result));
				}
#endif

#if NET7_0_OR_GREATER
                if (typeof(T).GetInterfaces().FirstOrDefault(i =>
                    i.IsConstructedGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IParsable<>) &&
                    i.GetGenericArguments()[0] == typeof(T)) is { } @interface)
                {
                    return typeof(T)
                        .GetInterfaceMap(@interface)
                        .GetImplementationMethod(@interface.GetMethod<TryParseProviderDelegate>(nameof(int.TryParse), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)!)
                        .CreateDelegate<TryParseProviderDelegate>();
                }
#endif

                if (GetParseMethod<TryParseProviderDelegate>(nameof(int.TryParse)) is { } method)
                    return method;

                if (GetParseMethod<TryParseNumberStylesProviderDelegate>(nameof(int.TryParse), out var defaultStyle) is { } numberStylesMethod)
                {
                    if (defaultStyle is null)
                        GetParseMethodInfo<ParseNumberStylesProviderDelegate>(nameof(int.Parse), out defaultStyle);

                    var style = defaultStyle ?? GetDefaultNumberStyles();
                    return ([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => numberStylesMethod(s, style, provider, out result);
                }

                if (GetParseMethod<TryParseDelegate>(nameof(int.TryParse)) is { } noProviderMethod)
                    return ([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => noProviderMethod(s, out result);

                return null;
            });
    }

    private static TryParseSpanProviderDelegate? GetTryParseSpanMethod()
    {
        return s_tryParseSpanMethodLazy.EnsureInitialized(
            static () =>
            {
                if (typeof(T) == typeof(char))
                {
                    return static (ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) =>
                        CharExtensions.TryParse(s, out UnsafeExtensions.OutAs<T, char>(out result));
                }

#if NETCOREAPP3_0_OR_GREATER
                if (typeof(T) == typeof(Rune))
                {
                    return static (ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) =>
						RuneExtensions.TryParse(s, out UnsafeExtensions.OutAs<T, Rune>(out result));
                }
#endif

#if NET7_0_OR_GREATER
                if (typeof(T).GetInterfaces().FirstOrDefault(i =>
                    i.IsConstructedGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ISpanParsable<>) &&
                    i.GetGenericArguments()[0] == typeof(T)) is { } @interface)
                {
                    return typeof(T)
                        .GetInterfaceMap(@interface)
                        .GetImplementationMethod(@interface.GetMethod<TryParseSpanProviderDelegate>(nameof(int.TryParse), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)!)
                        .CreateDelegate<TryParseSpanProviderDelegate>();
                }
#endif

                if (GetParseMethod<TryParseSpanProviderDelegate>(nameof(int.TryParse)) is { } method)
                    return method;

                if (GetParseMethod<TryParseSpanNumberStylesProviderDelegate>(nameof(int.TryParse), out var defaultStyle) is { } numberStylesMethod)
                {
                    if (defaultStyle is null)
                        GetParseMethodInfo<ParseSpanNumberStylesProviderDelegate>(nameof(int.Parse), out defaultStyle);

                    var style = defaultStyle ?? GetDefaultNumberStyles();
                    return (ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => numberStylesMethod(s, style, provider, out result);
                }

                if (GetParseMethod<TryParseSpanDelegate>(nameof(int.TryParse)) is { } noProviderMethod)
                    return (ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result) => noProviderMethod(s, out result);

                return null;
            });
    }

    private static NumberStyles GetDefaultNumberStyles()
    {
#if NET7_0_OR_GREATER
        var interfaces = typeof(T).GetInterfaces();
        if (interfaces.Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IFloatingPointIeee754<>)))
        {
            return NumberStyles.Float | NumberStyles.AllowThousands;
        }

        if (interfaces.Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IBinaryInteger<>)))
        {
            return NumberStyles.Integer;
        }
#endif

        if (typeof(T).GetMember(
            "NaN",
            MemberTypes.Field | MemberTypes.Property,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding)
            is [var member] && FieldOrPropertyInfo.Get(member) is { IsReadOnly: true } fieldOrProperty && fieldOrProperty.Type == typeof(T))
        {
            return NumberStyles.Float | NumberStyles.AllowThousands;
        }

        if (typeof(T).Name.StartsWith("Int", StringComparison.Ordinal, out var rest1) && !(rest1.Length > 0 && char.IsLower(rest1[0])) ||
            typeof(T).Name.StartsWith("UInt", StringComparison.Ordinal, out var rest2) && !(rest2.Length > 0 && char.IsLower(rest2[0])) ||
            typeof(T) == typeof(BigInteger))
        {
            return NumberStyles.Integer;
        }

        return NumberStyles.Number;
    }

    private static MethodInfo? GetParseMethodInfo<TDelegate>(string methodName)
        where TDelegate : Delegate
    {
        Debug.Assert(methodName is "Parse" or "TryParse");

        return typeof(T).GetMethod<TDelegate>(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
    }

    private static MethodInfo? GetParseMethodInfo<TDelegate>(string methodName, out NumberStyles? defaultNumberStyles)
        where TDelegate : Delegate
    {
        Debug.Assert(methodName is "Parse" or "TryParse");
        Debug.Assert(typeof(TDelegate).GetInvokeMethod().GetParameters()[1].ParameterType == typeof(NumberStyles));

        var method = GetParseMethodInfo<TDelegate>(methodName);
        if (method is null)
        {
            defaultNumberStyles = null;
            return null;
        }

        var numberStylesParameter = method.GetParameters()[1];
        Debug.Assert(numberStylesParameter.ParameterType == typeof(NumberStyles));

        defaultNumberStyles = numberStylesParameter.GetDefaultValue<NumberStyles>().AsNullable();

        return method;
    }

    private static TDelegate? GetParseMethod<TDelegate>(string methodName)
        where TDelegate : Delegate
    {
        var method = GetParseMethodInfo<TDelegate>(methodName);
        if (method is null)
            return null;

#if NET5_0_OR_GREATER
        return method.CreateDelegate<TDelegate>();
#else
        return (TDelegate)method.CreateDelegate(typeof(TDelegate));
#endif
    }

    private static TDelegate? GetParseMethod<TDelegate>(string methodName, out NumberStyles? defaultNumberStyles)
        where TDelegate : Delegate
    {
        var method = GetParseMethodInfo<TDelegate>(methodName, out defaultNumberStyles);
        if (method is null)
            return null;

#if NET5_0_OR_GREATER
        return method.CreateDelegate<TDelegate>();
#else
        return (TDelegate)method.CreateDelegate(typeof(TDelegate));
#endif
    }
}

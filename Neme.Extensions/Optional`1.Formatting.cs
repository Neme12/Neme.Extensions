using Neme.Extensions.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

public readonly partial struct Optional<T>
{
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

	public static Optional<T> Parse(string s) =>
		Parse(s, provider: null);

	public static Optional<T> Parse(string s, IFormatProvider? provider)
	{
		if (s is null)
			throw new ArgumentNullException(nameof(s));

		switch (ParseCore(s.AsSpan(), provider, out var optionalInside))
		{
			case ParseResult.None:
				return None;
			case ParseResult.Some:
                if (!optionalInside.TryGetValue(out var inside))
                    return new(default!);

                if (ParseHelper<T>.TryParseString(inside, out var stringValue))
                    return new(stringValue);

                if (ParseHelper<T>.GetParseSpanMethod() is { } spanMethod)
				{
					T result;

					try
					{
						result = spanMethod.Invoke(inside, provider);
					}
					catch (FormatException e)
					{
						return ThrowHelper.ThrowFormat<Optional<T>>(s, e);
					}

					return new(result);
				}

				if (ParseHelper<T>.GetParseMethod() is { } stringMethod)
				{
					var insideString = inside.ToString();
					T result;

                    try
					{
						result = stringMethod.Invoke(insideString, provider);
					}
					catch (FormatException e)
					{
						return ThrowHelper.ThrowFormat<Optional<T>>(s, e);
					}

					return result;
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
		switch (ParseCore(s, provider, out var optionalInside))
		{
			case ParseResult.None:
				return None;
			case ParseResult.Some:
                if (!optionalInside.TryGetValue(out var inside))
                    return new(default!);

                if (ParseHelper<T>.TryParseString(inside, out var stringValue))
                    return new(stringValue);

                if (ParseHelper<T>.GetParseSpanMethod() is { } spanMethod)
				{
					T result;

					try
					{
						result = spanMethod.Invoke(inside, provider);
					}
					catch (FormatException e)
					{
						return ThrowHelper.ThrowFormat<Optional<T>>(s, e);
					}

					return new(result);
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

		switch (ParseCore(s.AsSpan(), provider, out var optionalInside))
		{
			case ParseResult.None:
				result = None;
				return true;
			case ParseResult.Some:
                if (!optionalInside.TryGetValue(out var inside))
                {
                    result = new(default!);
					return true;
				}

				if (ParseHelper<T>.TryParseString(inside, out var stringValue))
				{
					result = new(stringValue);
					return true;
				}

                if (ParseHelper<T>.GetTryParseSpanMethod() is { } spanMethod)
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

				if (ParseHelper<T>.GetTryParseMethod() is { } stringMethod)
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
		switch (ParseCore(s, provider, out var optionalInside))
		{
			case ParseResult.None:
				result = None;
				return true;
			case ParseResult.Some:
                if (!optionalInside.TryGetValue(out var inside))
                {
                    result = new(default!);
					return true;
				}

                if (ParseHelper<T>.TryParseString(inside, out var stringValue))
                {
                    result = new(stringValue);
                    return true;
                }

                if (ParseHelper<T>.GetTryParseSpanMethod() is { } spanMethod)
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

    [DoesNotReturn]
	private static void ThrowNoParseMethod(string methodName) =>
		throw new NotSupportedException($"Type {typeof(T)} has no appropriate {methodName} method.");

	private enum ParseResult
	{
		Error,
		None,
		Some,
	}

	private static ParseResult ParseCore(ReadOnlySpan<char> s, IFormatProvider? provider, out OptionalReadOnlySpan<char> inside)
	{
		if (s.Equals("None".AsSpan(), StringComparison.Ordinal))
		{
			inside = default;
			return ParseResult.None;
		}

		const string prefix = "Some { ";
		const string suffix = " }";

		if (s.StartsWith(prefix.AsSpan(), StringComparison.Ordinal) && s.EndsWith(suffix.AsSpan(), StringComparison.Ordinal))
		{
			if (s.Length == prefix.Length + suffix.Length - 1)
			{
				inside = default;
				return ParseResult.Some;
			}

			inside = new(s[prefix.Length..^suffix.Length]);
			return ParseResult.Some;
		}

		inside = default;
		return ParseResult.Error;
	}
}

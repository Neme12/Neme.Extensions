using Neme.Extensions.Collections;
using Neme.Extensions.Contracts;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Neme.Extensions.Text;

public partial struct Grapheme
{
    public static bool IsValid(int value) =>
        Rune.IsValid(value);

    public static bool IsValid(uint value) =>
        Rune.IsValid(value);

    public static bool IsValid(char ch) =>
        RuneExtensions.IsValid(ch);

    public static bool IsValid(char highSurrogate, char lowSurrogate) =>
        RuneExtensions.IsValid(highSurrogate, lowSurrogate);

    public static Grapheme CreateFromUtf16(string chars)
    {
        return TryCreateFromUtf16Core(chars.AsSpan(), out var result) switch
        {
            CreateFromCharsResult.Empty => throw new ArgumentException(EmptyMessage, nameof(chars)),
            CreateFromCharsResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(chars)),
            CreateFromCharsResult.InvalidData => throw new ArgumentException(InvalidDataMessage, nameof(chars)),
            CreateFromCharsResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public static Grapheme CreateFromUtf16(ReadOnlySpan<char> chars)
    {
        return TryCreateFromUtf16Core(chars, out var result) switch
        {
            CreateFromCharsResult.Empty => throw new ArgumentException(EmptyMessage, nameof(chars)),
            CreateFromCharsResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(chars)),
            CreateFromCharsResult.InvalidData => throw new ArgumentException(InvalidDataMessage, nameof(chars)),
            CreateFromCharsResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public static Grapheme CreateFromUtf8(ReadOnlySpan<byte> bytes)
    {
        return TryCreateFromUtf8Core(bytes, out var result) switch
        {
            CreateFromCharsResult.Empty => throw new ArgumentException(EmptyMessage, nameof(bytes)),
            CreateFromCharsResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(bytes)),
            CreateFromCharsResult.InvalidData => throw new ArgumentException(InvalidDataMessage, nameof(bytes)),
            CreateFromCharsResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public static bool TryCreate(int value, out Grapheme result)
    {
        if (Rune.TryCreate(value, out var rune))
        {
            result = new(rune);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryCreate(uint value, out Grapheme result)
    {
        if (Rune.TryCreate(value, out var rune))
        {
            result = new(rune);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryCreate(char ch, out Grapheme result)
    {
        if (Rune.TryCreate(ch, out var rune))
        {
            result = new(rune);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryCreate(char highSurrogate, char lowSurrogate, out Grapheme result)
    {
        if (Rune.TryCreate(highSurrogate, lowSurrogate, out var rune))
        {
            result = new(rune);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryCreateFromUtf16(ReadOnlySpan<char> chars, out Grapheme result) =>
        TryCreateFromUtf16Core(chars, out result) is CreateFromCharsResult.Success;

    public static bool TryCreateFromUtf16(string? chars, out Grapheme result) =>
        TryCreateFromUtf16Core(chars.AsSpan(), out result) is CreateFromCharsResult.Success;

    public static bool TryCreateFromUtf8(ReadOnlySpan<byte> bytes, out Grapheme result) =>
        TryCreateFromUtf8Core(bytes, out result) is CreateFromCharsResult.Success;

    public static bool TryCreate(Rune[] runes, out Grapheme result) =>
        TryCreateCore(runes.AsSpan(), out result) is CreateFromRunesResult.Success;

    public static bool TryCreate(ImmutableArray<Rune> runes, out Grapheme result) =>
        TryCreateCore(runes, out result) is CreateFromRunesResult.Success;

    public static bool TryCreate(ReadOnlySpan<Rune> runes, out Grapheme result) =>
        TryCreateCore(runes, out result) is CreateFromRunesResult.Success;

    public static bool TryCreate(IEnumerable<Rune>? runes, out Grapheme result) =>
        TryCreateCore(runes, out result) is CreateFromRunesResult.Success;

    private enum CreateFromCharsResult
    {
        Success,
        Empty,
        MoreGraphemes,
        InvalidData,
    }

    private enum CreateFromRunesResult
    {
        Success,
        Empty,
        MoreGraphemes,
    }

    private static CreateFromCharsResult TryCreateFromUtf16Core(ReadOnlySpan<char> chars, out Grapheme result)
    {
        if (chars.IsEmpty)
        {
            result = default;
            return CreateFromCharsResult.Empty;
        }

        if (!IsSingleGrapheme(chars))
        {
            result = default;
            return CreateFromCharsResult.MoreGraphemes;
        }

        return DecodeFromUtf16Span(chars, out result) switch
        {
            DecodeResult.Success => CreateFromCharsResult.Success,
            DecodeResult.InvalidData => CreateFromCharsResult.InvalidData,
            _ => throw Assert.Unreachable(),
        };
    }

    private static CreateFromCharsResult TryCreateFromUtf8Core(ReadOnlySpan<byte> bytes, out Grapheme result)
    {
        if (bytes.IsEmpty)
        {
            result = default;
            return CreateFromCharsResult.Empty;
        }

        if (!IsSingleGrapheme(bytes))
        {
            result = default;
            return CreateFromCharsResult.MoreGraphemes;
        }

        return DecodeFromUtf8Span(bytes, out result) switch
        {
            DecodeResult.Success => CreateFromCharsResult.Success,
            DecodeResult.InvalidData => CreateFromCharsResult.InvalidData,
            _ => throw Assert.Unreachable(),
        };
    }

    private static CreateFromRunesResult TryCreateCore(ImmutableArray<Rune> runes, out Grapheme result)
    {
        var runeCount = runes.Length;
        if (runeCount == 0)
        {
            result = default;
            return CreateFromRunesResult.Empty;
        }

        if (!IsSingleGrapheme(runes.AsSpan()))
        {
            result = default;
            return CreateFromRunesResult.MoreGraphemes;
        }

        result = new Grapheme(_: default, SmallImmutableArray.Create(runes));
        return CreateFromRunesResult.Success;
    }

    private static CreateFromRunesResult TryCreateCore(ReadOnlySpan<Rune> runes, out Grapheme result)
    {
        var runeCount = runes.Length;
        if (runeCount == 0)
        {
            result = default;
            return CreateFromRunesResult.Empty;
        }

        if (!IsSingleGrapheme(runes))
        {
            result = default;
            return CreateFromRunesResult.MoreGraphemes;
        }

        result = new Grapheme(_: default, SmallImmutableArray.Create(runes));
        return CreateFromRunesResult.Success;
    }

    private static CreateFromRunesResult TryCreateCore(IEnumerable<Rune>? runes, out Grapheme result)
    {
        if (runes is null)
        {
            result = default;
            return CreateFromRunesResult.Empty;
        }

        var runesArray = runes.ToSmallImmutableArray();
        if (runesArray.Length == 0)
        {
            result = default;
            return CreateFromRunesResult.Empty;
        }

        if (!runesArray.WithSpan(IsSingleGrapheme))
        {
            result = default;
            return CreateFromRunesResult.MoreGraphemes;
        }

        result = new Grapheme(_: default, runesArray);
        return CreateFromRunesResult.Success;
    }

    private enum DecodeResult
    {
        Success,
        Empty,
        InvalidData,
    }

    public static Grapheme DecodeFromUtf16(ReadOnlySpan<char> source, out int charsConsumed)
    {
        return DecodeFromUtf16Core(source, out var result, out charsConsumed) switch
        {
            DecodeResult.Empty => throw new FormatException(InvalidDataMessage),
            DecodeResult.InvalidData => throw new FormatException(EmptyMessage),
            DecodeResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public static Grapheme DecodeFromUtf8(ReadOnlySpan<byte> source, out int bytesConsumed)
    {
        return DecodeFromUtf8Core(source, out var result, out bytesConsumed) switch
        {
            DecodeResult.Empty => throw new FormatException(InvalidDataMessage),
            DecodeResult.InvalidData => throw new FormatException(EmptyMessage),
            DecodeResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public static bool TryDecodeFromUtf16(ReadOnlySpan<char> source, out Grapheme result, out int charsConsumed) =>
        DecodeFromUtf16Core(source, out result, out charsConsumed) is DecodeResult.Success;

    public static bool TryDecodeFromUtf8(ReadOnlySpan<byte> source, out Grapheme result, out int bytesConsumed) =>
        DecodeFromUtf8Core(source, out result, out bytesConsumed) is DecodeResult.Success;

    private static DecodeResult DecodeFromUtf16Core(ReadOnlySpan<char> source, out Grapheme result, out int charsConsumed)
    {
        charsConsumed = 0;

        if (source.IsEmpty)
        {
            result = ReplacementChar;
            return DecodeResult.Empty;
        }

        var graphemeLength = StringInfoPolyfill.GetNextTextElementLength(source);
        var decodeResult = DecodeFromUtf16Span(source[..graphemeLength], out result);

        charsConsumed = graphemeLength;
        return decodeResult;
    }

    private static DecodeResult DecodeFromUtf8Core(ReadOnlySpan<byte> source, out Grapheme result, out int bytesConsumed)
    {
        bytesConsumed = 0;

        if (source.IsEmpty)
        {
            result = ReplacementChar;
            return DecodeResult.Empty;
        }

        var graphemeLength = StringInfoExtensions.GetNextTextElementLength(source);
        var decodeResult = DecodeFromUtf8Span(source[..graphemeLength], out result);

        bytesConsumed = graphemeLength;
        return decodeResult;
    }

    private static DecodeResult DecodeFromUtf16Span(ReadOnlySpan<char> source, out Grapheme result)
    {
        Debug.AssertNotEmpty(source);

        var charsConsumed = 0;

        var builder = SmallImmutableArray.CreateBuilder<Rune>(source.Length);
        var span = source;

        while (!span.IsEmpty)
        {
            var status = Rune.DecodeFromUtf16(span, out var rune, out var consumed);
            charsConsumed += consumed;

            if (status is not OperationStatus.Done)
            {
                result = ReplacementChar;
                return DecodeResult.InvalidData;
            }

            builder.Add(rune);

            span = span[consumed..];
        }

        Debug.AssertNotEmpty<Rune, SmallImmutableArray<Rune>.Builder>(in builder);

        result = new(_: default, builder.DrainToImmutable());
        return DecodeResult.Success;
    }

    private static DecodeResult DecodeFromUtf8Span(ReadOnlySpan<byte> source, out Grapheme result)
    {
        Debug.AssertNotEmpty(source);

        var bytesConsumed = 0;

        var builder = SmallImmutableArray.CreateBuilder<Rune>(source.Length);
        var span = source;

        while (!span.IsEmpty)
        {
            var status = Rune.DecodeFromUtf8(span, out var rune, out var consumed);
            bytesConsumed += consumed;

            if (status is not OperationStatus.Done)
            {
                result = ReplacementChar;
                return DecodeResult.InvalidData;
            }

            builder.Add(rune);

            span = span[consumed..];
        }

        Debug.AssertNotEmpty<Rune, SmallImmutableArray<Rune>.Builder>(in builder);

        result = new(_: default, builder.DrainToImmutable());
        return DecodeResult.Success;
    }

    public static Grapheme GetGraphemeAt(string input, int index)
    {
        var length = StringInfoPolyfill.GetNextTextElementLength(input, index);
        return CreateFromUtf16(input.AsSpan(index, length));
    }

    public static bool TryGetGraphemeAt(string input, int index, out Grapheme value)
    {
        var length = StringInfoPolyfill.GetNextTextElementLength(input, index);
        return TryCreateFromUtf16Core(input.AsSpan(index, length), out value) is CreateFromCharsResult.Success;
    }

#if NET7_0_OR_GREATER
    static Grapheme IParsable<Grapheme>.Parse(string s, IFormatProvider? provider)
    {
        return TryCreateFromUtf16Core(s, out var result) switch
        {
            CreateFromCharsResult.Empty => throw new FormatException(EmptyMessage),
            CreateFromCharsResult.MoreGraphemes => throw new FormatException(MoreGraphemesMessage),
            CreateFromCharsResult.InvalidData => throw new FormatException(InvalidDataMessage),
            CreateFromCharsResult.Success => result,
            _ => throw new UnreachableException(),
        };
    }

    static bool IParsable<Grapheme>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Grapheme result) =>
        TryCreateFromUtf16Core(s, out result) is CreateFromCharsResult.Success;

    static Grapheme ISpanParsable<Grapheme>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return TryCreateFromUtf16Core(s, out var result) switch
        {
            CreateFromCharsResult.Empty => throw new FormatException(EmptyMessage),
            CreateFromCharsResult.MoreGraphemes => throw new FormatException(MoreGraphemesMessage),
            CreateFromCharsResult.InvalidData => throw new FormatException(InvalidDataMessage),
            CreateFromCharsResult.Success => result,
            _ => throw new UnreachableException(),
        };
    }

    static bool ISpanParsable<Grapheme>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Grapheme result) =>
        TryCreateFromUtf16Core(s, out result) is CreateFromCharsResult.Success;
#endif

#if NET8_0_OR_GREATER
    static Grapheme IUtf8SpanParsable<Grapheme>.Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
    {
        return TryCreateFromUtf8Core(utf8Text, out var result) switch
        {
            CreateFromCharsResult.Empty => throw new FormatException(EmptyMessage),
            CreateFromCharsResult.MoreGraphemes => throw new FormatException(MoreGraphemesMessage),
            CreateFromCharsResult.InvalidData => throw new FormatException(InvalidDataMessage),
            CreateFromCharsResult.Success => result,
            _ => throw new UnreachableException(),
        };
    }

    static bool IUtf8SpanParsable<Grapheme>.TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, [MaybeNullWhen(false)] out Grapheme result) =>
        TryCreateFromUtf8Core(utf8Text, out result) is CreateFromCharsResult.Success;
#endif
}

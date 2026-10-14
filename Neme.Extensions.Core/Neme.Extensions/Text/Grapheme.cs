using Neme.Extensions.Collections;
using Neme.Extensions.Contracts;
using Roslyn.Utilities;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Neme.Extensions.Text;

public readonly partial struct Grapheme :
    IEquatable<Grapheme>,
    IComparable<Grapheme>,
    IComparable,
    IFormattable,
    IReadOnlyList<Rune>,
    ICloneable,
    IConvertible
#if NET6_0_OR_GREATER
    , ISpanFormattable
#endif
#if NET7_0_OR_GREATER
    , ISpanParsable<Grapheme>
    , IComparisonOperators<Grapheme, Grapheme, bool>
#endif
#if NET8_0_OR_GREATER
    , IUtf8SpanFormattable
    , IUtf8SpanParsable<Grapheme>
#endif
{
    [Obsolete("Use the property.")]
    internal readonly SmallImmutableArray<Rune> _runes;

    private const string EmptyMessage = "String is empty.";
    private const string MoreGraphemesMessage = "String contains more than one grapheme.";
    private const string InvalidDataMessage = "String contains invalid data.";

#pragma warning disable CS0618 // Type or member is obsolete
    public Grapheme(int value)
    {
        _runes = new(new Rune(value));
    }

    public Grapheme(uint value)
    {
        _runes = new(new Rune(value));
    }

    public Grapheme(char ch)
    {
        _runes = new(new Rune(ch));
    }

    public Grapheme(char highSurrogate, char lowSurrogate)
    {
        _runes = new(new Rune(highSurrogate, lowSurrogate));
    }

    public Grapheme(Rune rune)
    {
        _runes = new(rune);
    }

    public Grapheme(Rune[] runes)
    {
        this = TryCreateCore(runes.AsSpan(), out var result) switch
        {
            CreateFromRunesResult.Empty => throw new ArgumentException(EmptyMessage, nameof(runes)),
            CreateFromRunesResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(runes)),
            CreateFromRunesResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public Grapheme(ImmutableArray<Rune> runes)
    {
        this = TryCreateCore(runes, out var result) switch
        {
            CreateFromRunesResult.Empty => throw new ArgumentException(EmptyMessage, nameof(runes)),
            CreateFromRunesResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(runes)),
            CreateFromRunesResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public Grapheme(ReadOnlySpan<Rune> runes)
    {
        this = TryCreateCore(runes, out var result) switch
        {
            CreateFromRunesResult.Empty => throw new ArgumentException(EmptyMessage, nameof(runes)),
            CreateFromRunesResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(runes)),
            CreateFromRunesResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    public Grapheme(IEnumerable<Rune> runes)
    {
        this = TryCreateCore(runes, out var result) switch
        {
            CreateFromRunesResult.Empty => throw new ArgumentException(EmptyMessage, nameof(runes)),
            CreateFromRunesResult.MoreGraphemes => throw new ArgumentException(MoreGraphemesMessage, nameof(runes)),
            CreateFromRunesResult.Success => result,
            _ => throw Assert.Unreachable(),
        };
    }

    private Grapheme(bool _, SmallImmutableArray<Rune> runes)
    {
        Debug.AssertNotEmpty(runes);
        Debug.Assert(runes.WithSpan(IsSingleGrapheme));
        Debug.Assert(IsSingleGrapheme(string.Concat(runes).AsSpan()));

        _runes = runes;
    }

    private Grapheme(bool _, ReadOnlySpan<char> chars)
    {
        Debug.AssertEqual(TryCreateFromUtf16Core(chars, out var __), CreateFromCharsResult.Success);

        var builder = SmallImmutableArray.CreateBuilder<Rune>(chars.Length);

        foreach (var rune in chars.EnumerateRunes())
            builder.Add(rune);

        _runes = builder.DrainToImmutable();
    }

    internal SmallImmutableArray<Rune> Runes =>
        _runes.Length > 0 ? _runes : SmallImmutableArray.Create(default(Rune));

#pragma warning restore CS0618 // Type or member is obsolete

    public int Length =>
        Runes.Length;

    public Rune this[int index] =>
        Runes[index];

    public int Utf8SequenceLength
    {
        get
        {
            var length = 0;

            foreach (var rune in Runes)
                length += rune.Utf8SequenceLength;

            return length;
        }
    }

    public int Utf16SequenceLength
    {
        get
        {
            var length = 0;

            foreach (var rune in Runes)
                length += rune.Utf16SequenceLength;

            return length;
        }
    }

    public int Utf32SequenceLength =>
        Runes.Length;

    public bool IsAscii =>
        Runes is [{ IsAscii: true }];

    public bool IsBmp =>
        Runes is [{ IsBmp: true }];

    public bool IsSingleRune =>
        Runes is [_];

    private static bool IsSingleGrapheme(ReadOnlySpan<char> chars)
    {
        Debug.AssertNotEmpty(chars);

        return StringInfoPolyfill.GetNextTextElementLength(chars) == chars.Length;
    }

    private static bool IsSingleGrapheme(ReadOnlySpan<byte> utf8Bytes)
    {
        Debug.AssertNotEmpty(utf8Bytes);

        return StringInfoExtensions.GetNextTextElementLength(utf8Bytes) == utf8Bytes.Length;
    }

    private static bool IsSingleGrapheme(ReadOnlySpan<Rune> runes)
    {
        Debug.AssertNotEmpty(runes);

        return StringInfoExtensions.GetNextTextElementLength(runes) == runes.Length;
    }

    public bool Equals(Grapheme other) =>
        Runes.SequenceEqual(other.Runes);

    public int CompareTo(Grapheme other) =>
        Runes.SequenceCompareTo(other.Runes);

    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;

        if (obj is not Grapheme other)
            throw new ArgumentException(null, paramName: nameof(obj));

        return CompareTo(other);
    }

    public override bool Equals(object? obj) =>
        obj is Grapheme other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = 0;

        foreach (var rune in Runes)
            hashCode = HashCode.Combine(hashCode, rune.GetHashCode());

        return hashCode;
    }

    object ICloneable.Clone() =>
        this;

    int IReadOnlyCollection<Rune>.Count =>
        Length;

    public Enumerator GetEnumerator() =>
        new(this);

#pragma warning disable RS0042 // Do not copy value
    IEnumerator<Rune> IEnumerable<Rune>.GetEnumerator() =>
        GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
#pragma warning restore RS0042 // Do not copy value

    public static explicit operator Grapheme(int value) =>
        new(value);

    public static explicit operator Grapheme(uint value) =>
        new(value);

    public static explicit operator Grapheme(char ch) =>
        new(ch);

    public static implicit operator Grapheme(Rune rune) =>
        new(rune);

    public static explicit operator int(Grapheme grapheme)
    {
        return grapheme.Runes switch
        {
            [var rune] => rune.Value,
            _ => throw new InvalidCastException(),
        };
    }

    public static explicit operator uint(Grapheme grapheme)
    {
        return grapheme.Runes switch
        {
            [var rune] => unchecked((uint)rune.Value),
            _ => throw new InvalidCastException(),
        };
    }

    public static explicit operator char(Grapheme grapheme)
    {
        return grapheme.Runes switch
        {
            [{ Utf16SequenceLength: 1 } rune] => (char)rune.Value,
            _ => throw new InvalidCastException(),
        };
    }

    public static explicit operator Rune(Grapheme grapheme)
    {
        return grapheme.Runes switch
        {
            [var rune] => rune,
            _ => throw new InvalidCastException(),
        };
    }

    public static bool operator ==(Grapheme left, Grapheme right) =>
        left.Equals(right);

    public static bool operator !=(Grapheme left, Grapheme right) =>
        !(left == right);

    public static bool operator <(Grapheme left, Grapheme right) =>
        left.CompareTo(right) < 0;

    public static bool operator <=(Grapheme left, Grapheme right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >(Grapheme left, Grapheme right) =>
        left.CompareTo(right) > 0;

    public static bool operator >=(Grapheme left, Grapheme right) =>
        left.CompareTo(right) >= 0;

    public static Grapheme ReplacementChar =>
        new(Rune.ReplacementChar);

    [NonDefaultable]
    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IEnumerator<Rune>
    {
        private SmallImmutableArray<Rune>.Enumerator _enumerator;

        internal Enumerator(Grapheme grapheme)
        {
            _enumerator = grapheme.Runes.GetEnumerator();
        }

        public readonly Rune Current =>
            _enumerator.Current;

        readonly object? IEnumerator.Current =>
            Current;

        public bool MoveNext() =>
            _enumerator.MoveNext();

        public void Reset() =>
            _enumerator.Reset();

#pragma warning disable IDE0251 // Make member 'readonly'
        void IDisposable.Dispose()
#pragma warning restore IDE0251 // Make member 'readonly'
        {
        }
    }
}

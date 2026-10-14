using Neme.Extensions.Buffers;
using Neme.Extensions.Collections;
using Neme.Extensions.Contracts;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Neme.Extensions.Text;

public partial struct Grapheme
{
    public static Grapheme ToUpper(Grapheme value, CultureInfo? culture) =>
    TransformChars(value, culture, static (buffer, culture) => ((ReadOnlySpan<char>)buffer).ToUpper(buffer, culture));

    public static Grapheme ToLower(Grapheme value, CultureInfo? culture) =>
        TransformChars(value, culture, static (buffer, culture) => ((ReadOnlySpan<char>)buffer).ToLower(buffer, culture));

    public static Grapheme ToUpperInvariant(Grapheme value) =>
        TransformChars(value, false, static (buffer, _) => ((ReadOnlySpan<char>)buffer).ToUpperInvariant(buffer));

    public static Grapheme ToLowerInvariant(Grapheme value) =>
        TransformChars(value, false, static (buffer, _) => ((ReadOnlySpan<char>)buffer).ToLowerInvariant(buffer));

    private static Grapheme TransformChars<TState>(Grapheme value, TState state, SpanAction<char, TState> transform)
    {
        Debug.AssertNotNull(transform);

        return WithChars(value.Runes, state, (buffer, state) =>
        {
            transform(buffer, state);
            return new Grapheme(_: default, buffer);
        });
    }

    private static TResult WithChars<TState, TResult>(
        SmallImmutableArray<Rune> runes,
        TState state,
        SpanFunc<char, TState, TResult> action)
    {
        Debug.AssertNotNull(action);

        var bufferLength = runes.Length * 2;

        using (var bufferLease = ArrayPool<char>.Shared.RentLeaseOrStackalloc(
            bufferLength,
            bufferLength < 64 ? stackalloc char[bufferLength] : default))
        {
            var buffer = ToChars(runes, bufferLease.Buffer);
            return action(buffer, state);
        }

        static Span<char> ToChars(SmallImmutableArray<Rune> runes, Span<char> destination)
        {
            int position = 0;

            foreach (var rune in runes)
            {
                var length = rune.EncodeToUtf16(destination[position..]);
                position += length;
            }

            return destination[..position];
        }
    }
}

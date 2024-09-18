using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Neme.Extensions.Text;

#if NETCOREAPP3_0_OR_GREATER
internal static class RuneExtensions
{
    public static Rune Parse(string s)
    {
        if (s is null)
            throw new ArgumentNullException(nameof(s));

        return Parse(s.AsSpan());
    }

    public static Rune Parse(ReadOnlySpan<char> s)
    {
        if (Rune.DecodeFromUtf16(s, out var rune, out var charsConsumed) is not OperationStatus.Done || charsConsumed != s.Length)
            return ThrowHelper.ThrowFormat<Rune>(s.ToString());

        return rune;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out Rune result) =>
        TryParse(s.AsSpan(), out result);

    public static bool TryParse(ReadOnlySpan<char> s, out Rune result) =>
        Rune.DecodeFromUtf16(s, out result, out var charsConsumed) is OperationStatus.Done && charsConsumed == s.Length;
}
#endif

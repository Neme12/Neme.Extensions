using Neme.Extensions.Contracts;
using Roslyn.Utilities;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Text;

[NonDefaultable]
[StructLayout(LayoutKind.Auto)]
public struct StringGraphemeEnumerator : IEnumerable<Grapheme>, IEnumerator<Grapheme>
{
    private string _string;
    private Grapheme _current;
    private int _nextIndex;

    internal StringGraphemeEnumerator(string value)
    {
        Debug.AssertNotNull(value);

        _string = value;
        _current = default;
        _nextIndex = 0;
    }

    public readonly Grapheme Current
    {
        get
        {
            Require.NotDisposed(_string is null, this);
            Require.Positive(_nextIndex);

            return _current;
        }
    }

    readonly object? IEnumerator.Current =>
        Current;
    
    public bool MoveNext()
    {
        Require.NotDisposed(_string is null, this);

        Debug.AssertInRange(_nextIndex, 0, _string.Length);

        if (_nextIndex == _string.Length)
        {
            _current = default;
            return false;
        }

        if (!Grapheme.TryDecodeFromUtf16(_string.AsSpan(_nextIndex), out _current, out var charsConsumed))
            _current = Grapheme.ReplacementChar;

        _nextIndex += charsConsumed;
        return true;
    }

    public void Reset()
    {
        Require.NotDisposed(_string is null, this);

        _current = default;
        _nextIndex = 0;
    }

    public void Dispose()
    {
        _string = null!;
        _current = default;
    }

    public readonly StringGraphemeEnumerator GetEnumerator() =>
        this;

    readonly IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    readonly IEnumerator<Grapheme> IEnumerable<Grapheme>.GetEnumerator() =>
        GetEnumerator();
}

using Neme.Extensions.Contracts;
using Roslyn.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Text;

[NonDefaultable]
[StructLayout(LayoutKind.Auto)]
public ref struct SpanGraphemeEnumerator
{
    private ReadOnlySpan<char> _buffer;
    private Grapheme _current;
    private int _nextIndex;

    internal SpanGraphemeEnumerator(ReadOnlySpan<char> buffer)
    {
        _buffer = buffer;
        _current = default;
        _nextIndex = 0;
    }

    public readonly Grapheme Current
    {
        get
        {
            Require.NotDisposed(_nextIndex == -1, typeof(SpanGraphemeEnumerator));
            Require.Positive(_nextIndex);

            return _current;
        }
    }

    public bool MoveNext()
    {
        Require.NotDisposed(_nextIndex == -1, typeof(SpanGraphemeEnumerator));

        Debug.AssertInRange(_nextIndex, 0, _buffer.Length);

        if (_nextIndex == _buffer.Length)
        {
            _current = default;
            return false;
        }

        if (!Grapheme.TryDecodeFromUtf16(_buffer[_nextIndex..], out _current, out var charsConsumed))
            _current = Grapheme.ReplacementChar;

        _nextIndex += charsConsumed;
        return true;
    }

    public void Reset()
    {
        Require.NotDisposed(_nextIndex == -1, typeof(SpanGraphemeEnumerator));

        _current = default;
        _nextIndex = 0;
    }

    public void Dispose()
    {
        _buffer = default;
        _current = default;
        _nextIndex = -1;
    }

    public readonly SpanGraphemeEnumerator GetEnumerator() =>
        this;
}

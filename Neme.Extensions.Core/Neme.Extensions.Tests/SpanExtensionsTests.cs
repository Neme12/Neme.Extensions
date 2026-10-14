using Neme.Extensions.Tests.Utilities;

namespace Neme.Extensions.Tests;

public sealed class SpanExtensionsTests
{
    [Fact]
    public void String_Null()
    {
        Assert.Throws<ArgumentNullException>("value", () => new Span<char>().TryWrite((string)null!, out _));
    }

    [Fact]
    public void String_ExactLength()
    {
        Span<char> span = stackalloc char[5];
        Assert.True(span.TryWrite("hello", out var charsWritten));
        Assert.Equal(5, charsWritten);
        AssertPolyfill.Equal("hello".AsSpan(), span);
    }

    [Fact]
    public void String_Longer()
    {
        Span<char> span = stackalloc char[6];
        span[^1] = '1';
        Assert.True(span.TryWrite("hello", out var charsWritten));
        Assert.Equal(5, charsWritten);
        AssertPolyfill.Equal("hello1".AsSpan(), span);
    }

    [Fact]
    public void String_Shorter()
    {
        Span<char> span = stackalloc char[4];
        Assert.False(span.TryWrite("hello", out var charsWritten));
        Assert.Equal(0, charsWritten);
        AssertPolyfill.Equal("\0\0\0\0".AsSpan(), span);
    }

    [Fact]
    public void Span_ExactLength()
    {
        Span<char> span = stackalloc char[5];
        Assert.True(span.TryWrite("hello".AsSpan(), out var charsWritten));
        Assert.Equal(5, charsWritten);
        AssertPolyfill.Equal("hello".AsSpan(), span);
    }

    [Fact]
    public void Span_Longer()
    {
        Span<char> span = stackalloc char[6];
        span[^1] = '1';
        Assert.True(span.TryWrite("hello".AsSpan(), out var charsWritten));
        Assert.Equal(5, charsWritten);
        AssertPolyfill.Equal("hello1".AsSpan(), span);
    }

    [Fact]
    public void Span_Shorter()
    {
        Span<char> span = stackalloc char[4];
        Assert.False(span.TryWrite("hello".AsSpan(), out var charsWritten));
        Assert.Equal(0, charsWritten);
        AssertPolyfill.Equal("\0\0\0\0".AsSpan(), span);
    }
}

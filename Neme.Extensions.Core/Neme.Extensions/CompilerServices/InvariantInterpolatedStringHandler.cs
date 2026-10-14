using System.Globalization;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.CompilerServices;

[InterpolatedStringHandler]
public ref struct InvariantInterpolatedStringHandler
{
    private DefaultInterpolatedStringHandler _handler;

    public InvariantInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount, CultureInfo.InvariantCulture);
    }

    public readonly override string ToString() =>
        _handler.ToString();

    public string ToStringAndClear() =>
        _handler.ToStringAndClear();

#if NET10_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() =>
        _handler.Clear();

    public readonly ReadOnlySpan<char> Text =>
        _handler.Text;
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string value) =>
        _handler.AppendLiteral(value);

    #region AppendFormatted
    #region AppendFormatted T
    public void AppendFormatted<T>(T value) =>
        _handler.AppendFormatted(value);

    public void AppendFormatted<T>(T value, string? format) =>
        _handler.AppendFormatted(value, format);

    public void AppendFormatted<T>(T value, int alignment) =>
        _handler.AppendFormatted(value, alignment);

    public void AppendFormatted<T>(T value, int alignment, string? format) =>
        _handler.AppendFormatted(value, alignment, format);
    #endregion

    #region AppendFormatted ReadOnlySpan<char>
#if NET7_0_OR_GREATER
    public void AppendFormatted(scoped ReadOnlySpan<char> value) =>
        _handler.AppendFormatted(value);

    public void AppendFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null) =>
        _handler.AppendFormatted(value, alignment, format);
#else
    public void AppendFormatted(ReadOnlySpan<char> value) =>
        _handler.AppendFormatted(value);

    public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null) =>
        _handler.AppendFormatted(value, alignment, format);
#endif
    #endregion

    #region AppendFormatted string
    public void AppendFormatted(string? value) =>
        _handler.AppendFormatted(value);

    public void AppendFormatted(string? value, int alignment = 0, string? format = null) =>
        _handler.AppendFormatted(value, alignment, format);
    #endregion

    #region AppendFormatted object
    public void AppendFormatted(object? value, int alignment = 0, string? format = null) =>
        _handler.AppendFormatted(value, alignment, format);
    #endregion
#endregion
}

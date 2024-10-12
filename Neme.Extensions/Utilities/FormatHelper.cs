using System.Runtime.CompilerServices;

namespace Neme.Extensions.Utilities;

internal static class FormatHelper
{
#if NET6_0_OR_GREATER
    public static string Format(IFormatProvider? provider, [InterpolatedStringHandlerArgument(nameof(provider))] ref DefaultInterpolatedStringHandler handler) =>
        handler.ToStringAndClear();
#else
    public static string Format(IFormatProvider? provider, FormattableString formattable) =>
        formattable.ToString(provider);
#endif
}

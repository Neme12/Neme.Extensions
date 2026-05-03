using Neme.Extensions.CompilerServices;

namespace Neme.Extensions;

public static class InterpolatedString
{
    public static string Invariant(ref InvariantInterpolatedStringHandler handler) =>
        handler.ToStringAndClear();
}

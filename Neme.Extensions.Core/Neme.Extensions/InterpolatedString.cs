using Neme.Extensions.CompilerServices;

namespace Neme.Extensions;

#if NEME_EXTENSIONS
public
#else
internal
#endif
static class InterpolatedString
{
    public static string Invariant(ref InvariantInterpolatedStringHandler handler) =>
        handler.ToStringAndClear();
}

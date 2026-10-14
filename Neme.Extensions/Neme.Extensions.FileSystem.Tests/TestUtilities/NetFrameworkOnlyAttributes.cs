using Neme.Extensions.InteropServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.Tests.TestUtilities;

internal sealed class NetFrameworkOnlyFactAttribute : FactAttribute
{
    public NetFrameworkOnlyFactAttribute(string? reason = null)
    {
        if (!RuntimeInformation.IsNetFramework)
        {
            Skip = reason is not null
                ? InterpolatedString.Invariant($"This test only runs on .NET Framework. Reason: {reason}")
                : InterpolatedString.Invariant($"This test only runs on .NET Framework.");
        }
    }
}

internal sealed class NetFrameworkOnlyTheoryAttribute : TheoryAttribute
{
    public NetFrameworkOnlyTheoryAttribute(string? reason = null)
    {
        if (!RuntimeInformation.IsNetFramework)
        {
            Skip = reason is not null
                ? InterpolatedString.Invariant($"This test only runs on .NET Framework. Reason: {reason}")
                : InterpolatedString.Invariant($"This test only runs on .NET Framework.");
        }
    }
}

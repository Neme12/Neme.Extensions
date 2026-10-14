using System.Runtime.InteropServices;

namespace Neme.Extensions.Tests.Utilities;

internal enum Platform
{
    Windows,
    Linux,
    MacOS,
    Unix,
}

internal sealed class PlatformOnlyFactAttribute : FactAttribute
{
    public PlatformOnlyFactAttribute(params Platform[] platforms)
    {
        if (!platforms.Any(PlatformAttributeUtilities.IsPlatform))
        {
            Skip = $"This test only runs on the following platforms: {string.Join(", ", platforms)}.";
        }
    }
}

internal sealed class PlatformOnlyTheoryAttribute : TheoryAttribute
{
    public PlatformOnlyTheoryAttribute(params Platform[] platforms)
    {
        if (!platforms.Any(PlatformAttributeUtilities.IsPlatform))
        {
            Skip = $"This test only runs on the following platforms: {string.Join(", ", platforms)}.";
        }
    }
}

internal static class PlatformAttributeUtilities
{
    public static bool IsPlatform(Platform platform)
    {
        return platform switch
        {
            Platform.Windows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
            Platform.Linux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
            Platform.MacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX),
            Platform.Unix => !RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
            _ => throw new ArgumentException2(nameof(platform), platform, "Invalid platform specified."),
        };
    }
}

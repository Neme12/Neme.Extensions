using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.Tests.TestUtilities;

internal static class PathUtilities
{
    public static void AssertPathsEqual(string expected, string actual)
    {
        var normalizedExpected = NormalizeSymlinks(expected).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var normalizedActual = NormalizeSymlinks(actual).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        Assert.Equal(normalizedExpected, normalizedActual, ignoreCase: Path.DirectorySeparatorChar == '\\');
    }

    private static string NormalizeSymlinks(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            const string prefix = "/private";
            return path.StartsWith(prefix + "/var")
                ? path.Substring(prefix.Length)
                : path;
        }

        return path;
    }
}

using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem.Tests.TestUtilities;

internal static class PathUtilities
{
    public static string Normalize(string path)
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

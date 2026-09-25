namespace Neme.Extensions.FileSystem.Tests.TestUtilities;

internal static class PathUtilities
{
    public static string Normalize(string path) =>
        new DirectoryInfo(path).FullName;
}

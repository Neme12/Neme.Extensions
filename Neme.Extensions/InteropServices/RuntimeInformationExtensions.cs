using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class RuntimeInformationExtensions
{
    extension(RuntimeInformation)
    {
        public static bool IsNetVersionOrGreater(int major, int minor)
        {
            var (runtimeKind, versionMajor, versionMinor) = GetTargetRuntime();

            if (runtimeKind != RuntimeKind.Net)
                return false;

            return new Version(versionMajor, versionMinor) >= new Version(major, minor);
        }

        public static bool IsNetCoreVersionOrGreater(int major, int minor) =>
            IsNetVersionOrGreater(major, minor);

        public static bool IsNetFrameworkVersionOrGreater(int major, int minor)
        {
            var (runtimeKind, versionMajor, versionMinor) = GetTargetRuntime();

            if (runtimeKind != RuntimeKind.NetFramework)
                return false;

            return new Version(versionMajor, versionMinor) >= new Version(major, minor);
        }
    }

    private enum RuntimeKind
    {
        Net,
        NetFramework,
    }

    private static (RuntimeKind runtimeKind, int versionMajor, int versionMinor) GetTargetRuntime()
    {
        const string netFrameworkPrefix = ".NET Framework ";
        const string netCorePrefix = ".NET Core";
        const string netPrefix = ".NET";
        var frameworkDescription = RuntimeInformation.FrameworkDescription;

        var (runtimeKind, prefixLength) =
            frameworkDescription.StartsWith(netFrameworkPrefix, StringComparison.Ordinal) ? (RuntimeKind.NetFramework, netFrameworkPrefix.Length) :
            frameworkDescription.StartsWith(netCorePrefix, StringComparison.Ordinal) ? (RuntimeKind.Net, netCorePrefix.Length) :
            frameworkDescription.StartsWith(netPrefix, StringComparison.Ordinal) ? (RuntimeKind.Net, netPrefix.Length) :
            throw new PlatformNotSupportedException($"Unknown framework description: {frameworkDescription}");

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        var versionText = frameworkDescription.AsSpan(prefixLength);
#else
        var versionText = frameworkDescription.Substring(prefixLength);
#endif

        var version = Version.Parse(versionText);
        return (runtimeKind, version.Major, version.Minor);
    }
}

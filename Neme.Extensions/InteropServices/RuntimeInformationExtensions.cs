using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class RuntimeInformationExtensions
{
    extension(RuntimeInformation)
    {
        public static bool IsNetCore =>
            GetRuntimeKindAndPrefixLength(RuntimeInformation.FrameworkDescription).runtimeKind == RuntimeKind.NetCore;

        public static bool IsNetCoreVersionOrGreater(int major, int minor)
        {
            var (runtimeKind, versionMajor, versionMinor) = GetTargetRuntime(RuntimeInformation.FrameworkDescription);

            if (runtimeKind != RuntimeKind.NetCore)
                return false;

            return new Version(versionMajor, versionMinor) >= new Version(major, minor);
        }


        public static bool IsNetFrameworkVersionOrGreater(int major, int minor)
        {
            var (runtimeKind, versionMajor, versionMinor) = GetTargetRuntime(RuntimeInformation.FrameworkDescription);

            if (runtimeKind != RuntimeKind.NetFramework)
                return false;

            return new Version(versionMajor, versionMinor) >= new Version(major, minor);
        }
    }

    private const string NetFrameworkPrefix = ".NET Framework ";
    private const string NetCorePrefix = ".NET Core";
    private const string NetPrefix = ".NET";

    private enum RuntimeKind
    {
        NetCore,
        NetFramework,
    }

    private static (RuntimeKind runtimeKind, int versionMajor, int versionMinor) GetTargetRuntime(string frameworkDescription)
    {
        var (runtimeKind, prefixLength) = GetRuntimeKindAndPrefixLength(frameworkDescription);

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        var versionText = frameworkDescription.AsSpan(prefixLength);
#else
        var versionText = frameworkDescription.Substring(prefixLength);
#endif

        var version = Version.Parse(versionText);
        return (runtimeKind, version.Major, version.Minor);
    }

    private static (RuntimeKind runtimeKind, int prefixLength) GetRuntimeKindAndPrefixLength(string frameworkDescription)
    {
        return
            frameworkDescription.StartsWith(NetFrameworkPrefix, StringComparison.Ordinal) ? (RuntimeKind.NetFramework, NetFrameworkPrefix.Length) :
            frameworkDescription.StartsWith(NetCorePrefix, StringComparison.Ordinal) ? (RuntimeKind.NetCore, NetCorePrefix.Length) :
            frameworkDescription.StartsWith(NetPrefix, StringComparison.Ordinal) ? (RuntimeKind.NetCore, NetPrefix.Length) :
            throw new PlatformNotSupportedException($"Unknown framework description: {frameworkDescription}");
    }
}

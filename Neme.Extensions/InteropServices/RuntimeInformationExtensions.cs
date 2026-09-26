using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.InteropServices;

#if NEME_EXTENSIONS
public
#else
internal
#endif
enum FrameworkKind
{
    Unknown,
    NetCore,
    NetFramework,
}

#if NEME_EXTENSIONS
public
#else
internal
#endif
static class RuntimeInformationExtensions
{
    extension(RuntimeInformation)
    {
        public static bool IsNetCore =>
            GetTargetFramework().frameworkKind == FrameworkKind.NetCore;

        public static bool IsNetFramework =>
            GetTargetFramework().frameworkKind == FrameworkKind.NetFramework;

        public static bool IsFramework(FrameworkKind framework) =>
            GetTargetFramework().frameworkKind == framework;

        public static bool IsNetCoreVersionOrGreater(int major, int minor)
        {
            var (frameworkKind, frameworkVersion) = GetTargetFramework();
            return frameworkKind == FrameworkKind.NetCore && frameworkVersion >= new Version(major, minor);
        }

        public static bool IsNetFrameworkVersionOrGreater(int major, int minor)
        {
            var (frameworkKind, frameworkVersion) = GetTargetFramework();
            return frameworkKind == FrameworkKind.NetFramework && frameworkVersion >= new Version(major, minor);
        }

        public static bool IsFrameworkVersionOrGreater(FrameworkKind framework, int major, int minor)
        {
            var (frameworkKind, frameworkVersion) = GetTargetFramework();
            return frameworkKind == framework && frameworkVersion >= new Version(major, minor);
        }
    }

    private const string NetFrameworkDescriptionPrefix = ".NET Framework ";
    private const string NetFrameworkIdentifier = ".NETFramework";
    private const string NetCoreIdentifier = ".NETCoreApp";

    private static (FrameworkKind frameworkKind, Version? frameworkVersion) GetTargetFramework()
    {
        var bclAssembly = typeof(object).Assembly;

        var targetFrameworkAttribute = bclAssembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (targetFrameworkAttribute is null)
        {
            var frameworkDescription = RuntimeInformation.FrameworkDescription;
            if (frameworkDescription.StartsWith(NetFrameworkDescriptionPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var versionText = frameworkDescription.Substring(NetFrameworkDescriptionPrefix.Length);
                if (Version.TryParse(versionText, out var version))
                    return (FrameworkKind.NetFramework, version);
            }

            return (FrameworkKind.Unknown, null);
        }

        var frameworkName = new FrameworkName(targetFrameworkAttribute.FrameworkName);
       
        var frameworkKind = frameworkName.Identifier switch
        {
            NetCoreIdentifier => FrameworkKind.NetCore,
            NetFrameworkIdentifier => FrameworkKind.NetFramework,
            _ => FrameworkKind.Unknown,
        };

        return (frameworkKind, frameworkName.Version);
    }
}

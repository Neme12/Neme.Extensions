using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Diagnostics.CodeAnalysis;

namespace Neme.Extensions;

internal static class ThrowHelper
{
    private static readonly ResourceManager _coreAssemblyResourceManager =
        GetCoreAssemblyResourceManager();

    [DoesNotReturn]
    public static void ThrowNotSupported_ReadOnlyCollection() =>
        throw new NotSupportedException(_coreAssemblyResourceManager.GetString("NotSupported_ReadOnlyCollection"));

    private static ResourceManager GetCoreAssemblyResourceManager()
    {
        var assembly = typeof(object).Assembly;
        var prefix = assembly.GetName().Name + ".";
        const string suffix = ".resources";

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (resourceName.StartsWith(prefix, StringComparison.Ordinal) &&
                resourceName.EndsWith(suffix, StringComparison.Ordinal))
            {
                var baseName = resourceName[0..^suffix.Length];
                return new ResourceManager(baseName, assembly);
            }
        }

        throw new MissingManifestResourceException("Could not find the resource manager for the core assembly.");
    }
}

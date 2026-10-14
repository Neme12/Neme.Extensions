using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Neme.Extensions.SourceGenerators;

internal static class FrameworkNameExtensions
{
    extension([NotNullWhen(true)] FrameworkName? frameworkName)
    {
        public bool IsNetCore
        {
            get
            {
                return
                    frameworkName is not null &&
                    frameworkName.Identifier.Equals(".NETCoreApp", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}

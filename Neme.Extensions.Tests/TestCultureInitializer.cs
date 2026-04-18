using System.Globalization;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Tests;

internal static class TestCultureInitializer
{
#pragma warning disable CA2255
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Initialize()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }
}

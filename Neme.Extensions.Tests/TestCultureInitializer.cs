using System.Globalization;
#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

namespace Neme.Extensions.Tests;

internal static class TestCultureInitializer
{
#if NET5_0_OR_GREATER
#pragma warning disable CA2255
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Initialize()
#else
    // For .NET Framework, this needs to be called manually or via IClassFixture/ICollectionFixture
    // Since we can't use ModuleInitializer on .NET Framework 4.8
    internal static void Initialize()
#endif
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }
}

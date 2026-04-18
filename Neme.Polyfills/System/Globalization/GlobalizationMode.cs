// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Globalization;

internal static partial class GlobalizationMode
{
    // Split from GlobalizationMode so the whole class can be trimmed when Invariant=true. Trimming tests
    // validate this implementation detail.
    private static partial class Settings
    {
        internal static bool Invariant { get; } = AppContextConfigHelper.GetBooleanConfig("System.Globalization.Invariant", "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT");
    }

    // Note: Invariant=true and Invariant=false are substituted at different levels in the ILLink.Substitutions file.
    // This allows for the whole Settings nested class to be trimmed when Invariant=true, and allows for the Settings
    // static cctor (on Unix) to be preserved when Invariant=false.
    internal static bool Invariant => Settings.Invariant;
}
#endif

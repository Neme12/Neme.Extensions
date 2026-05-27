// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Neme.Extensions.Internal;

internal static class AppContextConfigHelper
{
    internal static bool GetBooleanConfig(string switchName, bool defaultValue) =>
        AppContext.TryGetSwitch(switchName, out bool value) ? value : defaultValue;

    internal static bool GetBooleanConfig(string switchName, string envVariable, bool defaultValue = false)
    {
        string? str = Environment.GetEnvironmentVariable(envVariable);
        if (str != null)
        {
            if (str == "1" || str.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (str == "0" || str.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return GetBooleanConfig(switchName, defaultValue);
    }
}

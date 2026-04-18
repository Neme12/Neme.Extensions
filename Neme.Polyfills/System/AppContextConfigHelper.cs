// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NETCOREAPP3_0_OR_GREATER
namespace System;

internal static class AppContextConfigHelper
{
    internal static bool GetBooleanConfig(string switchName, string envVariable, bool defaultValue = false)
    {
        if (!AppContext.TryGetSwitch(switchName, out bool ret))
        {
            string? switchValue = Environment.GetEnvironmentVariable(envVariable);
            ret = switchValue != null ? (BooleanPolyfill.IsTrueStringIgnoreCase(switchValue.AsSpan()) || switchValue.Equals("1", StringComparison.Ordinal)) : defaultValue;
        }

        return ret;
    }
}
#endif

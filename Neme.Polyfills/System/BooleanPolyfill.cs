// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NETCOREAPP3_0_OR_GREATER
using System.Runtime.InteropServices;

namespace System;

internal static class BooleanPolyfill
{
    internal static bool IsTrueStringIgnoreCase(ReadOnlySpan<char> value)
    {
        // "true" as a ulong, each char |'d with 0x0020 for case-insensitivity
        ulong true_val = BitConverter.IsLittleEndian ? 0x65007500720074ul : 0x74007200750065ul;
        return value.Length == 4 &&
               (MemoryMarshal.Read<ulong>(MemoryMarshal.AsBytes(value)) | 0x0020002000200020) == true_val;
    }
}
#endif

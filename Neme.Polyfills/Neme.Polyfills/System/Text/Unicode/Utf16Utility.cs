// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NETCOREAPP3_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace System.Text.Unicode;

internal static partial class Utf16Utility
{
    /// <summary>
    /// Returns true iff the UInt32 represents two ASCII UTF-16 characters in machine endianness.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool AllCharsInUInt32AreAscii(uint value)
    {
        return (value & ~0x007F_007Fu) == 0;
    }

    /// <summary>
    /// Given a UInt32 that represents two ASCII UTF-16 characters, returns the invariant
    /// lowercase representation of those characters. Requires the input value to contain
    /// two ASCII UTF-16 characters in machine endianness.
    /// </summary>
    /// <remarks>
    /// This is a branchless implementation.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ConvertAllAsciiCharsInUInt32ToLowercase(uint value)
    {
        // ASSUMPTION: Caller has validated that input value is ASCII.
        Debug.Assert(AllCharsInUInt32AreAscii(value));

        // the 0x80 bit of each word of 'lowerIndicator' will be set iff the word has value >= 'A'
        uint lowerIndicator = value + 0x0080_0080u - 0x0041_0041u;

        // the 0x80 bit of each word of 'upperIndicator' will be set iff the word has value > 'Z'
        uint upperIndicator = value + 0x0080_0080u - 0x005B_005Bu;

        // the 0x80 bit of each word of 'combinedIndicator' will be set iff the word has value >= 'A' and <= 'Z'
        uint combinedIndicator = (lowerIndicator ^ upperIndicator);

        // the 0x20 bit of each word of 'mask' will be set iff the word has value >= 'A' and <= 'Z'
        uint mask = (combinedIndicator & 0x0080_0080u) >> 2;

        return value ^ mask; // bit flip uppercase letters [A-Z] => [a-z]
    }

    /// <summary>
    /// Given a UInt32 that represents two ASCII UTF-16 characters, returns the invariant
    /// uppercase representation of those characters. Requires the input value to contain
    /// two ASCII UTF-16 characters in machine endianness.
    /// </summary>
    /// <remarks>
    /// This is a branchless implementation.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint ConvertAllAsciiCharsInUInt32ToUppercase(uint value)
    {
        // Intrinsified in mono interpreter
        // ASSUMPTION: Caller has validated that input value is ASCII.
        Debug.Assert(AllCharsInUInt32AreAscii(value));

        // the 0x80 bit of each word of 'lowerIndicator' will be set iff the word has value >= 'a'
        uint lowerIndicator = value + 0x0080_0080u - 0x0061_0061u;

        // the 0x80 bit of each word of 'upperIndicator' will be set iff the word has value > 'z'
        uint upperIndicator = value + 0x0080_0080u - 0x007B_007Bu;

        // the 0x80 bit of each word of 'combinedIndicator' will be set iff the word has value >= 'a' and <= 'z'
        uint combinedIndicator = (lowerIndicator ^ upperIndicator);

        // the 0x20 bit of each word of 'mask' will be set iff the word has value >= 'a' and <= 'z'
        uint mask = (combinedIndicator & 0x0080_0080u) >> 2;

        return value ^ mask; // bit flip lowercase letters [a-z] => [A-Z]
    }
}
#endif

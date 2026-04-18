// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Runtime.CompilerServices;

namespace System.Globalization;

/// <summary>
/// This class implements a set of methods for retrieving character type
/// information. Character type information is independent of culture
/// and region.
/// </summary>
internal static partial class CharUnicodeInfoPolyfill
{
    /*
     * GetGraphemeBreakClusterType
     * ===========================
     * Data derived from https://unicode.org/reports/tr29/#Default_Grapheme_Cluster_Table. Represents
     * grapheme cluster boundary information for the given code point.
     */

    internal static GraphemeClusterBreakType GetGraphemeClusterBreakType(Rune rune)
    {
        nuint offset = GetNumericGraphemeTableOffsetNoBoundsChecks((uint)rune.Value);
        return (GraphemeClusterBreakType)Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(GraphemeSegmentationValues), offset);
    }

    /*
     * HELPER AND TABLE LOOKUP ROUTINES
     */

    /// <summary>
    /// Retrieves the offset into the "NumericGrapheme" arrays where this code point's
    /// information is stored. Used for getting numeric information and grapheme boundary
    /// information.
    /// </summary>
    private static nuint GetNumericGraphemeTableOffsetNoBoundsChecks(uint codePoint)
    {
        UnicodeDebug.AssertIsValidCodePoint(codePoint);

        // The code below is written with the assumption that the backing store is 11:5:4.
        AssertNumericGraphemeTableLevels(11, 5, 4);

        // Get the level index item from the high 11 bits of the code point.

        uint index = Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(NumericGraphemeLevel1Index), codePoint >> 9);

        // Get the level 2 WORD offset from the next 5 bits of the code point.
        // This provides the base offset of the level 3 table.
        // Note that & has lower precedence than +, so remember the parens.

        ref byte level2Ref = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(NumericGraphemeLevel2Index), (index << 6) + ((codePoint >> 3) & 0b_0011_1110));

        if (BitConverter.IsLittleEndian)
        {
            index = Unsafe.ReadUnaligned<ushort>(ref level2Ref);
        }
        else
        {
            index = BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<ushort>(ref level2Ref));
        }

        // Get the result from the low 4 bits of the code point.
        // This is the offset into the values table where the data is stored.

        return Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(NumericGraphemeLevel3Index), (index << 4) + (codePoint & 0x0F));
    }
}
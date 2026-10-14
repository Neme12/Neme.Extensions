// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    internal static partial class SpanHelpers // .Byte
    {
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
        internal static unsafe int IndexOfNullByte(byte* searchSpace)
        {
            for (int i = 0; ; ++i)
            {
                if (searchSpace[i] == 0)
                    return i;
            }
        }
#endif
    }
}

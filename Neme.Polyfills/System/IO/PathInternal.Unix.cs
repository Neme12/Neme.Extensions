// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace System.IO;

#if !NETCOREAPP3_0_OR_GREATER

internal static partial class PathInternal
{
    internal static class Unix
    {
        private const char DirectorySeparatorChar = '/';
        private const char AltDirectorySeparatorChar = '/';

        internal static int GetRootLength(ReadOnlySpan<char> path)
        {
            return path.Length > 0 && IsDirectorySeparator(path[0]) ? 1 : 0;
        }

        internal static bool IsDirectorySeparator(char c)
        {
            // The alternate directory separator char is the same as the directory separator,
            // so we only need to check one.
            Debug.Assert(DirectorySeparatorChar == AltDirectorySeparatorChar);
            return c == DirectorySeparatorChar;
        }
    }
}

#endif

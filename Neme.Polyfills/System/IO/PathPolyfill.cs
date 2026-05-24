// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license

namespace System.IO;

#if !NETCOREAPP3_0_OR_GREATER

public static class PathPolyfill
{
    extension(Path)
    {
        /// <summary>
        /// Trims one trailing directory separator beyond the root of the path.
        /// </summary>
        public static string TrimEndingDirectorySeparator(string path) => PathInternal.TrimEndingDirectorySeparator(path);

        /// <summary>
        /// Trims one trailing directory separator beyond the root of the path.
        /// </summary>
        public static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) => PathInternal.TrimEndingDirectorySeparator(path);
    }
}

#endif

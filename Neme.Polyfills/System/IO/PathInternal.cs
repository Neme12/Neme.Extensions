// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.IO;

#if !NETCOREAPP3_0_OR_GREATER

internal static partial class PathInternal
{
    internal static bool IsDirectorySeparator(char c) =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            Windows.IsDirectorySeparator(c) :
            Unix.IsDirectorySeparator(c);

    internal static int GetRootLength(ReadOnlySpan<char> path) =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            Windows.GetRootLength(path) :
            Unix.GetRootLength(path);

    internal static bool IsRoot(ReadOnlySpan<char> path)
        => path.Length == GetRootLength(path);

    /// <summary>
    /// Returns true if the path ends in a directory separator.
    /// </summary>
    internal static bool EndsInDirectorySeparator([NotNullWhen(true)] string? path) =>
          !string.IsNullOrEmpty(path) && IsDirectorySeparator(path[^1]);

    /// <summary>
    /// Returns true if the path ends in a directory separator.
    /// </summary>
    internal static bool EndsInDirectorySeparator(ReadOnlySpan<char> path) =>
        path.Length > 0 && IsDirectorySeparator(path[^1]);

    /// <summary>
    /// Trims one trailing directory separator beyond the root of the path.
    /// </summary>
    [return: NotNullIfNotNull(nameof(path))]
    internal static string? TrimEndingDirectorySeparator(string? path) =>
        EndsInDirectorySeparator(path) && !IsRoot(path.AsSpan()) ?
            path.Substring(0, path.Length - 1) :
            path;

    /// <summary>
    /// Trims one trailing directory separator beyond the root of the path.
    /// </summary>
    internal static ReadOnlySpan<char> TrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
        EndsInDirectorySeparator(path) && !IsRoot(path) ?
            path.Slice(0, path.Length - 1) :
            path;
}

#endif

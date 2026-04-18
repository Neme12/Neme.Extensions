using Microsoft.Win32;
using Neme.Extensions.Utilities;
using System.Diagnostics;

namespace Neme.Extensions.Win32;


public readonly struct RegistryPath
{
    /// <summary>
    /// Windows native NT APIs have a maximum path length of 32,767 characters for registry paths.<br />
    /// Win32 APIs and many apps have a maximum length limit of 255, but we'll follow the true underlying OS limit.
    /// </summary>
    private const int MaxLength = short.MaxValue;

    private readonly RegistryHive _hive;
    private readonly string _hiveRelativePath;

    public RegistryPath(ReadOnlySpan<char> path)
    {
        if (path.IsEmpty)
            throw new ArgumentException("Registry path cannot be empty.", nameof(path));

        if (path.Length > MaxLength)
            throw new ArgumentException($"Registry path exceeds maximum length of {MaxLength} characters (provided: {path.Length}).", nameof(path));

        var firstSlash = path.IndexOf('\\');
        var hiveStart = 0;
        var hiveSegment = path[hiveStart..firstSlash];

        if (hiveSegment.Equals("Computer", StringComparison.OrdinalIgnoreCase))
        {
            var secondSlash = path[(firstSlash + 1)..].IndexOf('\\');
            hiveStart = secondSlash + 1;
            hiveSegment = path[hiveStart..secondSlash];
        }

        var hive = ParseHive(hiveSegment);
        if (hive is null)
            throw new ArgumentException($"Invalid hive name: {hiveSegment}", nameof(path));

        var relativePath = GetTrimmedNormalizedPath(path[(hiveStart + hiveSegment.Length + 1)..]);

        _hive = hive.Value;
        _hiveRelativePath = relativePath;
    }

    public RegistryPath(RegistryHive hive, ReadOnlySpan<char> relativePath)
    {
        if (hive is not (
            RegistryHive.ClassesRoot or
            RegistryHive.CurrentUser or
            RegistryHive.LocalMachine or
            RegistryHive.Users or
            RegistryHive.CurrentConfig or
            RegistryHive.PerformanceData))
        {
            throw new ArgumentException($"Invalid registry hive: {hive}", nameof(hive));
        }

        if (relativePath.Length > MaxLength)
            throw new ArgumentException($"Registry path exceeds maximum length of {MaxLength} characters (provided: {relativePath.Length}).", nameof(relativePath));

        _hive = hive;
        _hiveRelativePath = GetTrimmedNormalizedPath(relativePath);
    }

    public RegistryPath(RegistryHive hive, string relativePath)
    {
        if (hive is not (
            RegistryHive.ClassesRoot or
            RegistryHive.CurrentUser or
            RegistryHive.LocalMachine or
            RegistryHive.Users or
            RegistryHive.CurrentConfig or
            RegistryHive.PerformanceData))
        {
            throw new ArgumentException($"Invalid registry hive: {hive}", nameof(hive));
        }

        if (relativePath.Length > MaxLength)
            throw new ArgumentException($"Registry path exceeds maximum length of {MaxLength} characters (provided: {relativePath.Length}).", nameof(relativePath));

        _hive = hive;
        _hiveRelativePath = NeedsNormalizationOrTrimming(relativePath)
            ? GetTrimmedNormalizedPath(relativePath)
            : relativePath;
    }

    private static bool NeedsNormalizationOrTrimming(ReadOnlySpan<char> path)
    {
        return path.StartsWith('\\') ||
               path.EndsWith('\\') ||
               NeedsNormalization(path);
    }

    private static unsafe string GetTrimmedNormalizedPath(ReadOnlySpan<char> path)
    {
        while (path.StartsWith('\\'))
            path = path[1..];

        while (path.EndsWith('\\'))
            path = path[..^1];

        if (NeedsNormalization(path))
        {
            var buffer = (Span<char>)stackalloc char[path.Length];
            var written = NormalizePath(path, buffer);

#pragma warning disable CS9080 // stackalloc'ed buffers are method-scoped, not block-scoped like variables.
            path = buffer[..written];
#pragma warning restore CS9080
        }

        return path.ToString();
    }

    private static bool NeedsNormalization(ReadOnlySpan<char> path) =>
        path.Contains("\\\\", StringComparison.Ordinal);

    private static int NormalizePath(ReadOnlySpan<char> path, Span<char> destination)
    {
        // Using IndexOf to find slashes is faster than a manual loop over each char because it is vectorized.

        var written = 0;
        var window = path;

        while (true)
        {
            var slashIndex = window.IndexOf('\\');
            if (slashIndex == -1)
            {
                window.CopyTo(destination[written..]);
                written += window.Length;
                break;
            }

            var afterSlashIndex = slashIndex + 1;

            while (afterSlashIndex < window.Length && window[afterSlashIndex] == '\\')
                ++afterSlashIndex;

            window[..slashIndex].CopyTo(destination[written..]);
            written += slashIndex;

            destination[written] = '\\';
            written += 1;

            window = window[afterSlashIndex..];
        }

        return written;
    }

    /// <summary>
    /// Opens the registry key specified by this path.
    /// </summary>
    /// <param name="writable">true to open the key with write access; otherwise, false for read-only access.</param>
    /// <param name="view">The registry view to use (Default, Registry32, or Registry64).</param>
    /// <returns>The registry key, or null if the key does not exist.</returns>
    /// <remarks>
    /// The returned RegistryKey must be disposed when no longer needed.
    /// </remarks>
    public RegistryKey? OpenKey(bool writable = false, RegistryView view = RegistryView.Default)
    {
        using var baseKey = view == RegistryView.Default
            ? OwnedOrBorrowed.CreateBorrowed(GetHiveKey(_hive))
            : OwnedOrBorrowed.CreateOwned(RegistryKey.OpenBaseKey(_hive, view));

        if (_hiveRelativePath is null or "")
        {
            // Opening the hive root itself
            if (baseKey.OwnsValue)
            {
                // Return the newly opened base key without disposing it
                return baseKey.Move();
            }
            else
            {
                // Can't return the cached static instance directly
                // Open it properly with the requested access
                return RegistryKey.OpenBaseKey(_hive, view);
            }
        }

        return baseKey.Value.OpenSubKey(_hiveRelativePath, writable);
    }

    private static RegistryKey GetHiveKey(RegistryHive hive)
    {
        return hive switch
        {
            RegistryHive.ClassesRoot => Registry.ClassesRoot,
            RegistryHive.CurrentUser => Registry.CurrentUser,
            RegistryHive.LocalMachine => Registry.LocalMachine,
            RegistryHive.Users => Registry.Users,
            RegistryHive.CurrentConfig => Registry.CurrentConfig,
            RegistryHive.PerformanceData => Registry.PerformanceData,
            _ => throw new UnreachableException()
        };
    }

    private static RegistryHive? ParseHive(ReadOnlySpan<char> hiveName)
    {
        return
            hiveName.Equals("HKEY_CLASSES_ROOT", StringComparison.OrdinalIgnoreCase) ||
            hiveName.Equals("HKCR", StringComparison.OrdinalIgnoreCase)
                ? RegistryHive.ClassesRoot :
            hiveName.Equals("HKEY_CURRENT_USER", StringComparison.OrdinalIgnoreCase) ||
            hiveName.Equals("HKCU", StringComparison.OrdinalIgnoreCase)
                ? RegistryHive.CurrentUser :
            hiveName.Equals("HKEY_LOCAL_MACHINE", StringComparison.OrdinalIgnoreCase) ||
            hiveName.Equals("HKLM", StringComparison.OrdinalIgnoreCase)
                ? RegistryHive.LocalMachine :
            hiveName.Equals("HKEY_USERS", StringComparison.OrdinalIgnoreCase) ||
            hiveName.Equals("HKU", StringComparison.OrdinalIgnoreCase)
                ? RegistryHive.Users :
            hiveName.Equals("HKEY_CURRENT_CONFIG", StringComparison.OrdinalIgnoreCase) ||
            hiveName.Equals("HKCC", StringComparison.OrdinalIgnoreCase) // This abbreviation is not a first class citizen in Windows, but let's be more permissive.
                ? RegistryHive.CurrentConfig :
            hiveName.Equals("HKEY_PERFORMANCE_DATA", StringComparison.OrdinalIgnoreCase) || // Legacy and virtual hive, not recognized by RegEdit.
            hiveName.Equals("HKPD", StringComparison.OrdinalIgnoreCase)
                ? RegistryHive.PerformanceData
                : null;

    }

    private static string HiveToString(RegistryHive hive)
    {
        return hive switch
        {
            RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
            RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
            RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
            RegistryHive.Users => "HKEY_USERS",
            RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
            RegistryHive.PerformanceData => "HKEY_PERFORMANCE_DATA",
            _ => throw new UnreachableException(),
        };
    }

    private static string HiveToShortString(RegistryHive hive)
    {
        return hive switch
        {
            RegistryHive.ClassesRoot => "HKCR",
            RegistryHive.CurrentUser => "HKCU",
            RegistryHive.LocalMachine => "HKLM",
            RegistryHive.Users => "HKU",
            RegistryHive.CurrentConfig => "HKCC",
            RegistryHive.PerformanceData => "HKPD",
            _ => throw new UnreachableException(),
        };
    }

    private static string HiveToNativeString(RegistryHive hive)
    {
        return hive switch
        {
            RegistryHive.ClassesRoot => "\\Registry\\Machine\\Software\\Classes",
            RegistryHive.CurrentUser => "\\Registry\\User",
            RegistryHive.LocalMachine => "\\Registry\\Machine",
            RegistryHive.Users => "\\Registry\\User",
            RegistryHive.CurrentConfig => "\\Registry\\Machine\\System\\CurrentControlSet\\Hardware Profiles\\Current",
            RegistryHive.PerformanceData => throw new InvalidOperationException("The PerformanceData hive is a virtual hive that does not have a corresponding NT path."),
            _ => throw new UnreachableException(),
        };
    }


    /// <summary>
    /// Returns the hive of the registry path.
    /// </summary>
    public RegistryHive Hive => _hive;

    /// <summary>
    /// Returns the key to the hive of the registry path.
    /// </summary>
    public RegistryKey HiveKey => GetHiveKey(_hive);


    /// <summary>
    /// Returns the path relative to the hive.
    /// </summary>
    public string HiveRelativePath => _hiveRelativePath;

    /// <summary>
    /// Returns the path including the hive.
    /// </summary>
    public string Path
    {
        get
        {
            var hiveName = HiveToString(_hive);

            return _hiveRelativePath.Length > 0
                ? $"{hiveName}\\{_hiveRelativePath}"
                : hiveName;
        }
    }

    /// <summary>
    /// Returns the path with the abbreviated name for the hive.
    /// </summary>
    public string ShortPath
    {
        get
        {
            var hiveName = HiveToShortString(_hive);

            return _hiveRelativePath.Length > 0
                ? $"{hiveName}\\{_hiveRelativePath}"
                : hiveName;

        }
    }

    /// <summary>
    /// Returns the path as it appears in RegEdit, including the "Computer" prefix.
    /// </summary>
    public string FullyQualifiedPath
    {
        get
        {
            var hiveName = HiveToString(_hive);

            return _hiveRelativePath.Length > 0
                ? $"Computer\\{hiveName}\\{_hiveRelativePath}"
                : $"Computer\\{hiveName}";

        }
    }

    /// <summary>
    /// Returns the path as would be recognized by Windows native NT APIs.
    /// </summary>
    public string NativePath
    {
        get
        {
            var hiveName = HiveToNativeString(_hive);

            return _hiveRelativePath.Length > 0
                ? $"{hiveName}\\{_hiveRelativePath}"
                : hiveName;
        }
    }


    public override string ToString()
    {
        return Path;
    }
}

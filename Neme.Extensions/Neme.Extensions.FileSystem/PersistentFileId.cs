using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly partial record struct PersistentFileId : IEquatable<PersistentFileId>
#if NET7_0_OR_GREATER
        , IParsable<PersistentFileId>
#endif
{
    private readonly PlatformId _platformId;

    private PersistentFileId(WindowsId windowsFileId)
    {
        _platformId = windowsFileId;
    }

    private PersistentFileId(LinuxId linuxFileId)
    {
        _platformId = linuxFileId;
    }

    internal static PersistentFileId FromWindowsId(WindowsId windowsFileId) =>
        new PersistentFileId(windowsFileId);

    internal static PersistentFileId FromLinuxId(LinuxId linuxFileId) =>
        new PersistentFileId(linuxFileId);

    internal WindowsId WindowsFileId
    {
        get
        {
            if (_platformId is not WindowsId windowsId)
                throw new InvalidOperationException();

            return windowsId;
        }
    }

    internal LinuxId LinuxFileId
    {
        get
        {
            if (_platformId is not LinuxId linuxId)
                throw new InvalidOperationException();

            return linuxId;
        }
    }

    public bool Equals(PersistentFileId other)
    {
        if (_platformId?.GetType() != other._platformId?.GetType())
            return false;

        return _platformId switch
        {
            WindowsId windowsId => windowsId.Equals(other.WindowsFileId),
            LinuxId linuxId => linuxId.Equals(other.LinuxFileId),
            null => other._platformId is null,
            _ => throw new UnreachableException(),
        };
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = new HashCode();
            hashCode.Add(_platformId.GetType());

            switch (_platformId)
            {
                case WindowsId windowsId:
                    hashCode.Add(windowsId);
                    break;

                case LinuxId linuxId:
                    hashCode.Add(linuxId);
                    break;

                case null:
                    break;
            }

            return hashCode.ToHashCode();
        }
    }

    public static PersistentFileId Parse(string s)
    {
        ArgumentNullException.ThrowIfNull(s);

        var parts = s.Split(':');
        if (parts.Length < 2 ||
            parts[0] != "v1" ||
            parts[1].Length != 1)
        {
            throw new FormatException("Invalid PersistentFileId format.");
        }

        return parts[1] switch
        {
            "w" => new(WindowsId.Parse(parts)),
            "l" => new(LinuxId.Parse(parts)),
            _ => throw new FormatException("Unknown platform identifier in PersistentFileId."),
        };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out PersistentFileId result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        try
        {
            result = Parse(s!);
            return true;

        }
        catch (FormatException)
        {
            result = default;
            return false;
        }
    }

#if NET7_0_OR_GREATER
    static PersistentFileId IParsable<PersistentFileId>.Parse(string s, IFormatProvider? provider) =>
        Parse(s);

    static bool IParsable<PersistentFileId>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PersistentFileId result) =>
        TryParse(s, out result);
#endif

    public override string ToString() =>
        _platformId switch
        {
            WindowsId windowsId => windowsId.ToString(),
            LinuxId linuxId => linuxId.ToString(),
            _ => throw new InvalidOperationException("File ID is not initialized."),
        };

    internal abstract record class PlatformId
    {
    }
}

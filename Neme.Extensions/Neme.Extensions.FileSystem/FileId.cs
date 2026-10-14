using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Explicit)]
public readonly partial record struct FileId : IEquatable<FileId>
{
    [FieldOffset(0)]
    private readonly PlatformKind _platformKind;

    [FieldOffset(8)]
    private readonly WindowsId _windowsId;

    [FieldOffset(8)]
    private readonly UnixId _unixId;

    internal enum PlatformKind
    {
        None = 0,
        Windows = 1,
        Unix = 2,
    }

    private FileId(WindowsId windowsFileId)
    {
        _windowsId = windowsFileId;
        _platformKind = PlatformKind.Windows;
    }

    private FileId(UnixId unixFileId)
    {
        _unixId = unixFileId;
        _platformKind = PlatformKind.Unix;
    }

    internal static FileId FromWindowsId(WindowsId windowsFileId) =>
        new(windowsFileId);

    internal static FileId FromUnixId(UnixId unixFileId) =>
        new(unixFileId);

    internal WindowsId WindowsFileId
    {
        get
        {
            if (_platformKind != PlatformKind.Windows)
                throw new InvalidOperationException();

            return _windowsId;
        }
    }

    internal UnixId UnixFileId
    {
        get
        {
            if (_platformKind != PlatformKind.Unix)
                throw new InvalidOperationException();

            return _unixId;
        }
    }

    public bool Equals(FileId other)
    {
        if (_platformKind != other._platformKind)
            return false;

        return _platformKind switch
        {
            PlatformKind.Windows => _windowsId.Equals(other._windowsId),
            PlatformKind.Unix => _unixId.Equals(other._unixId),
            PlatformKind.None => true,
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = new HashCode();
            hashCode.Add(_platformKind);

            switch (_platformKind)
            {
                case PlatformKind.Windows:
                    hashCode.Add(_windowsId);
                    break;

                case PlatformKind.Unix:
                    hashCode.Add(_unixId);
                    break;
            }

            return hashCode.ToHashCode();
        }
    }
}

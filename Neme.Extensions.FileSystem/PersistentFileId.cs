using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Explicit)]
public readonly partial record struct PersistentFileId : IEquatable<PersistentFileId>
{
    [FieldOffset(0)]
    private readonly FileIdKind _fileIdKind;

    [FieldOffset(8)]
    private readonly WindowsId _windowsFileId;
    [FieldOffset(8)]
    private readonly LinuxId _linuxFileId;

    private enum FileIdKind : byte
    {
        Windows = 0,
        Linux = 1,
    }

    private PersistentFileId(WindowsId windowsFileId)
    {
        _fileIdKind = FileIdKind.Windows;
        _windowsFileId = windowsFileId;
    }

    private PersistentFileId(LinuxId linuxFileId)
    {
        _fileIdKind = FileIdKind.Linux;
        _linuxFileId = linuxFileId;
    }

    internal static PersistentFileId FromWindowsId(WindowsId windowsFileId) =>
        new PersistentFileId(windowsFileId);

    internal static PersistentFileId FromLinuxId(LinuxId linuxFileId) =>
        new PersistentFileId(linuxFileId);

    internal WindowsId WindowsFileId
    {
        get
        {
            if (_fileIdKind != FileIdKind.Windows)
                throw new InvalidOperationException();

            return _windowsFileId;
        }
    }

    internal LinuxId LinuxFileId
    {
        get
        {
            if (_fileIdKind != FileIdKind.Linux)
                throw new InvalidOperationException();

            return _linuxFileId;
        }
    }

    public bool Equals(PersistentFileId other)
    {
        if (_fileIdKind != other._fileIdKind)
            return false;

        return _fileIdKind switch
        {
            FileIdKind.Windows => WindowsFileId.Equals(other.WindowsFileId),
            FileIdKind.Linux => LinuxFileId.Equals(other.LinuxFileId),
            _ => throw new UnreachableException(),
        };
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_fileIdKind);

        switch (_fileIdKind)
        {
            case FileIdKind.Windows:
                hashCode.Add(_windowsFileId);
                break;

            case FileIdKind.Linux:
                hashCode.Add(_linuxFileId);
                break;

            default:
                throw new UnreachableException();
        }

        return hashCode.ToHashCode();
    }

    public override string ToString() =>
        _fileIdKind switch
        {
            FileIdKind.Windows => _windowsFileId.ToString(),
            FileIdKind.Linux => _linuxFileId.ToString(),
            _ => throw new UnreachableException(),
        };
}

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FsFileOptions
{
    private readonly FileAttributes _attributes;
    private readonly byte _mode;
    private readonly byte _access;
    private readonly byte _share;
    private readonly byte _options;
    private readonly ushort _unixCreateMode;

    public FsFileOptions(FileMode mode, FsFileAccess access, FileShare share)
    {
        _mode = (byte)mode;
        _access = (byte)access;
        _share = (byte)share;
    }

    public FsFileOptions(FileMode mode, FsFileAccess access)
    {
        _mode = (byte)mode;
        _access = (byte)access;
        _share = (byte)((access & FsFileAccess.Write) != 0 || (access & FsFileAccess.Delete) != 0
            ? FileShare.None
            : FileShare.Read);
    }

    public FileMode Mode
    {
        get => (FileMode)_mode;
        init => _mode = (byte)value;
    }

    public FsFileAccess Access
    {
        get => (FsFileAccess)_access;
        init => _access = (byte)value;
    }

    public FileShare Share
    {
        get => (FileShare)_share;
        init => _share = (byte)value;
    }

    public FileOptions Options
    {
        get => FileOptionsFromByte(_options);
        init => _options = FileOptionsToByte(value);
    }

    public FileAttributes Attributes
    {
        get => _attributes;
        init => _attributes = value;
    }

    public UnixFileMode? UnixCreateMode
    {
        get => _unixCreateMode != 0
            ? (UnixFileMode?)(_unixCreateMode - 1)
            : null;
        [UnsupportedOSPlatform("windows")]
        init
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException(Strings.PlatformNotSupported_UnixFileMode);

            _unixCreateMode = value.HasValue
                ? (ushort)(value.Value + 1)
                : (ushort)0;
        }
    }

#if NET6_0_OR_GREATER
    public static FsFileOptions FromFileStreamOptions(FileStreamOptions options)
    {
#if NET7_0_OR_GREATER
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
        {
            return new FsFileOptions
            {
                Mode = options.Mode,
                Access = FsFileAccess.FromFileAccess(options.Access),
                Share = options.Share,
                Options = options.Options,
            };
        }
#if NET7_0_OR_GREATER
        else
        {
            return new FsFileOptions
            {
                Mode = options.Mode,
                Access = FsFileAccess.FromFileAccess(options.Access),
                Share = options.Share,
                Options = options.Options,
                UnixCreateMode = options.UnixCreateMode,
            };
        }
#endif
    }

    public static implicit operator FsFileOptions(FileStreamOptions options) =>
        FromFileStreamOptions(options);
#endif

    private static byte FileOptionsToByte(FileOptions options)
    {
        byte value = 0;

        if ((options & FileOptions.Encrypted) != 0)
            value |= 1 << 0;

        if ((options & FileOptions.DeleteOnClose) != 0)
            value |= 1 << 1;

        if ((options & FileOptions.SequentialScan) != 0)
            value |= 1 << 2;

        if ((options & FileOptions.RandomAccess) != 0)
            value |= 1 << 3;

        if ((options & FileOptions.Asynchronous) != 0)
            value |= 1 << 4;

        if ((options & FileOptions.WriteThrough) != 0)
            value |= 1 << 5;

        return value;
    }

    private static FileOptions FileOptionsFromByte(byte value)
    {
        FileOptions options = 0;

        if ((value & (1 << 0)) != 0)
            options |= FileOptions.Encrypted;

        if ((value & (1 << 1)) != 0)
            options |= FileOptions.DeleteOnClose;

        if ((value & (1 << 2)) != 0)
            options |= FileOptions.SequentialScan;

        if ((value & (1 << 3)) != 0)
            options |= FileOptions.RandomAccess;

        if ((value & (1 << 4)) != 0)
            options |= FileOptions.Asynchronous;

        if ((value & (1 << 5)) != 0)
            options |= FileOptions.WriteThrough;

        return options;
    }
}

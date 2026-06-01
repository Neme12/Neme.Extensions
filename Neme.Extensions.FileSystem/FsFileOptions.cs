using Neme.Extensions.Contracts;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FsFileOptions
{
    private readonly AllOptions _allOptions;
    private readonly long _preallocationSize;

    public FsFileOptions(FileMode mode, FsFileAccess access, FileShare share)
    {
        _allOptions = ToAllOptions(mode, access, share, 0, 0, null);
    }

    public FsFileOptions(FileMode mode, FsFileAccess access)
    {
        _allOptions = ToAllOptions(mode, access, (access & FsFileAccess.Write) != 0 || (access & FsFileAccess.Delete) != 0
            ? FileShare.None
            : FileShare.Read, 0, 0, null);
    }

    public FileMode Mode
    {
        get => AllOptionsToFileMode(_allOptions);
        init => _allOptions = ToAllOptions(value, Access, Share, Options, Attributes, UnixCreateMode);
    }

    public FsFileAccess Access
    {
        get => AllOptionsToFsFileAccess(_allOptions);
        init => _allOptions = ToAllOptions(Mode, value, Share, Options, Attributes, UnixCreateMode);
    }

    public FileShare Share
    {
        get => AllOptionsToFileShare(_allOptions);
        init => _allOptions = ToAllOptions(Mode, Access, value, Options, Attributes, UnixCreateMode);
    }

    public FileOptions Options
    {
        get => AllOptionsToFileOptions(_allOptions);
        init => _allOptions = ToAllOptions(Mode, Access, Share, value, Attributes, UnixCreateMode);
    }

    public FileAttributes Attributes
    {
        get => AllOptionsToFileAttributes(_allOptions);
        init => _allOptions = ToAllOptions(Mode, Access, Share, Options, value, UnixCreateMode);
    }

    public UnixFileMode? UnixCreateMode
    {
        get => AllOptionsToUnixFileMode(_allOptions);
        [UnsupportedOSPlatform("windows")]
        init
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException(Strings.PlatformNotSupported_UnixFileMode);

            _allOptions = ToAllOptions(Mode, Access, Share, Options, Attributes, value);
        }
    }

    public long PreallocationSize
    {
        get => _preallocationSize;
        init
        {
            Require.ArgumentNotNegative(value);

            _preallocationSize = value;
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

    private static AllOptions ToAllOptions(
        FileMode mode,
        FsFileAccess access,
        FileShare share,
        FileOptions options,
        FileAttributes attributes,
        UnixFileMode? unixFileMode)
    {
        return FileModeToAllOptions(mode)
            | FsFileAccessToAllOptions(access)
            | FileShareToAllOptions(share)
            | FileOptionsToAllOptions(options)
            | FileAttributesToAllOptions(attributes)
            | UnixFileModeToAllOptions(unixFileMode);
    }

    private static AllOptions FileModeToAllOptions(FileMode mode)
    {
        return (AllOptions)((ulong)mode & ModeMask);
    }

    private static FileMode AllOptionsToFileMode(AllOptions options)
    {
        return (FileMode)((ulong)options & ModeMask);
    }

    private static AllOptions FsFileAccessToAllOptions(FsFileAccess access)
    {
        AllOptions value = 0;

        var rawAccess = (RawFsFileAccess)access;

        if ((rawAccess & RawFsFileAccess.ReadAttributes) != 0)
            value |= AllOptions.Access_ReadAttributes;

        if ((rawAccess & RawFsFileAccess.WriteAttributes) != 0)
            value |= AllOptions.Access_WriteAttributes;

        if ((rawAccess & RawFsFileAccess.Read) != 0)
            value |= AllOptions.Access_Read;

        if ((rawAccess & RawFsFileAccess.Write) != 0)
            value |= AllOptions.Access_Write;

        if ((rawAccess & RawFsFileAccess.Delete) != 0)
            value |= AllOptions.Access_Delete;

        if ((rawAccess & RawFsFileAccess.Execute) != 0)
            value |= AllOptions.Access_Execute;

        return value;
    }

    private static FsFileAccess AllOptionsToFsFileAccess(AllOptions options)
    {
        RawFsFileAccess value = 0;

        if ((options & AllOptions.Access_ReadAttributes) != 0)
            value |= RawFsFileAccess.ReadAttributes;

        if ((options & AllOptions.Access_WriteAttributes) != 0)
            value |= RawFsFileAccess.WriteAttributes;

        if ((options & AllOptions.Access_Read) != 0)
            value |= RawFsFileAccess.Read;

        if ((options & AllOptions.Access_Write) != 0)
            value |= RawFsFileAccess.Write;

        if ((options & AllOptions.Access_Delete) != 0)
            value |= RawFsFileAccess.Delete;

        if ((options & AllOptions.Access_Execute) != 0)
            value |= RawFsFileAccess.Execute;

        return (FsFileAccess)value;
    }

    private static AllOptions FileShareToAllOptions(FileShare share)
    {
        AllOptions value = 0;

        if ((share & FileShare.Read) != 0)
            value |= AllOptions.Share_Read;

        if ((share & FileShare.Write) != 0)
            value |= AllOptions.Share_Write;

        if ((share & FileShare.Delete) != 0)
            value |= AllOptions.Share_Delete;

        if ((share & FileShare.Inheritable) != 0)
            value |= AllOptions.Share_Inheritable;

        return value;
    }

    private static FileShare AllOptionsToFileShare(AllOptions options)
    {
        FileShare value = 0;

        if ((options & AllOptions.Share_Read) != 0)
            value |= FileShare.Read;

        if ((options & AllOptions.Share_Write) != 0)
            value |= FileShare.Write;

        if ((options & AllOptions.Share_Delete) != 0)
            value |= FileShare.Delete;

        if ((options & AllOptions.Share_Inheritable) != 0)
            value |= FileShare.Inheritable;

        return value;
    }

    private static AllOptions FileOptionsToAllOptions(FileOptions options)
    {
        AllOptions value = 0;

        if ((options & FileOptions.WriteThrough) != 0)
            value |= AllOptions.Options_WriteThrough;

        if ((options & FileOptions.Asynchronous) != 0)
            value |= AllOptions.Options_Asynchronous;

        if ((options & FileOptions.RandomAccess) != 0)
            value |= AllOptions.Options_RandomAccess;

        if ((options & FileOptions.DeleteOnClose) != 0)
            value |= AllOptions.Options_DeleteOnClose;

        if ((options & FileOptions.SequentialScan) != 0)
            value |= AllOptions.Options_SequentialScan;

        if ((options & FileOptions.Encrypted) != 0)
            value |= AllOptions.Options_Encrypted;

        return value;
    }

    private static FileOptions AllOptionsToFileOptions(AllOptions options)
    {
        FileOptions value = 0;

        if ((options & AllOptions.Options_WriteThrough) != 0)
            value |= FileOptions.WriteThrough;

        if ((options & AllOptions.Options_Asynchronous) != 0)
            value |= FileOptions.Asynchronous;

        if ((options & AllOptions.Options_RandomAccess) != 0)
            value |= FileOptions.RandomAccess;

        if ((options & AllOptions.Options_DeleteOnClose) != 0)
            value |= FileOptions.DeleteOnClose;

        if ((options & AllOptions.Options_SequentialScan) != 0)
            value |= FileOptions.SequentialScan;

        if ((options & AllOptions.Options_Encrypted) != 0)
            value |= FileOptions.Encrypted;

        return value;
    }

    private static AllOptions FileAttributesToAllOptions(FileAttributes attributes)
    {
        AllOptions value = 0;

        if ((attributes & FileAttributes.ReadOnly) != 0)
            value |= AllOptions.Attributes_ReadOnly;

        if ((attributes & FileAttributes.Hidden) != 0)
            value |= AllOptions.Attributes_Hidden;

        if ((attributes & FileAttributes.System) != 0)
            value |= AllOptions.Attributes_System;

        if ((attributes & FileAttributes.Directory) != 0)
            value |= AllOptions.Attributes_Directory;

        if ((attributes & FileAttributes.Archive) != 0)
            value |= AllOptions.Attributes_Archive;

        if ((attributes & FileAttributes.Device) != 0)
            value |= AllOptions.Attributes_Device;

        if ((attributes & FileAttributes.Normal) != 0)
            value |= AllOptions.Attributes_Normal;

        if ((attributes & FileAttributes.Temporary) != 0)
            value |= AllOptions.Attributes_Temporary;

        if ((attributes & FileAttributes.SparseFile) != 0)
            value |= AllOptions.Attributes_SparseFile;

        if ((attributes & FileAttributes.ReparsePoint) != 0)
            value |= AllOptions.Attributes_ReparsePoint;

        if ((attributes & FileAttributes.Compressed) != 0)
            value |= AllOptions.Attributes_Compressed;

        if ((attributes & FileAttributes.Offline) != 0)
            value |= AllOptions.Attributes_Offline;

        if ((attributes & FileAttributes.NotContentIndexed) != 0)
            value |= AllOptions.Attributes_NotContentIndexed;

        if ((attributes & FileAttributes.Encrypted) != 0)
            value |= AllOptions.Attributes_Encrypted;

        if ((attributes & FileAttributes.IntegrityStream) != 0)
            value |= AllOptions.Attributes_IntegrityStream;

        if ((attributes & FileAttributes.NoScrubData) != 0)
            value |= AllOptions.Attributes_NoScrubData;

        return value;
    }

    private static FileAttributes AllOptionsToFileAttributes(AllOptions options)
    {
        FileAttributes value = 0;

        if ((options & AllOptions.Attributes_ReadOnly) != 0)
            value |= FileAttributes.ReadOnly;

        if ((options & AllOptions.Attributes_Hidden) != 0)
            value |= FileAttributes.Hidden;

        if ((options & AllOptions.Attributes_System) != 0)
            value |= FileAttributes.System;

        if ((options & AllOptions.Attributes_Directory) != 0)
            value |= FileAttributes.Directory;

        if ((options & AllOptions.Attributes_Archive) != 0)
            value |= FileAttributes.Archive;

        if ((options & AllOptions.Attributes_Device) != 0)
            value |= FileAttributes.Device;

        if ((options & AllOptions.Attributes_Normal) != 0)
            value |= FileAttributes.Normal;

        if ((options & AllOptions.Attributes_Temporary) != 0)
            value |= FileAttributes.Temporary;

        if ((options & AllOptions.Attributes_SparseFile) != 0)
            value |= FileAttributes.SparseFile;

        if ((options & AllOptions.Attributes_ReparsePoint) != 0)
            value |= FileAttributes.ReparsePoint;

        if ((options & AllOptions.Attributes_Compressed) != 0)
            value |= FileAttributes.Compressed;

        if ((options & AllOptions.Attributes_Offline) != 0)
            value |= FileAttributes.Offline;

        if ((options & AllOptions.Attributes_NotContentIndexed) != 0)
            value |= FileAttributes.NotContentIndexed;

        if ((options & AllOptions.Attributes_Encrypted) != 0)
            value |= FileAttributes.Encrypted;

        if ((options & AllOptions.Attributes_IntegrityStream) != 0)
            value |= FileAttributes.IntegrityStream;

        if ((options & AllOptions.Attributes_NoScrubData) != 0)
            value |= FileAttributes.NoScrubData;

        return value;
    }

    private static AllOptions UnixFileModeToAllOptions(UnixFileMode? unixFileMode)
    {
        if (unixFileMode is null)
            return default;

        AllOptions value = AllOptions.UnixFileMode_NotNull;

        if ((unixFileMode & UnixFileMode.OtherExecute) != 0)
            value |= AllOptions.UnixFileMode_OtherExecute;

        if ((unixFileMode & UnixFileMode.OtherWrite) != 0)
            value |= AllOptions.UnixFileMode_OtherWrite;

        if ((unixFileMode & UnixFileMode.OtherRead) != 0)
            value |= AllOptions.UnixFileMode_OtherRead;

        if ((unixFileMode & UnixFileMode.GroupExecute) != 0)
            value |= AllOptions.UnixFileMode_GroupExecute;

        if ((unixFileMode & UnixFileMode.GroupWrite) != 0)
            value |= AllOptions.UnixFileMode_GroupWrite;

        if ((unixFileMode & UnixFileMode.GroupRead) != 0)
            value |= AllOptions.UnixFileMode_GroupRead;

        if ((unixFileMode & UnixFileMode.UserExecute) != 0)
            value |= AllOptions.UnixFileMode_UserExecute;

        if ((unixFileMode & UnixFileMode.UserWrite) != 0)
            value |= AllOptions.UnixFileMode_UserWrite;

        if ((unixFileMode & UnixFileMode.UserRead) != 0)
            value |= AllOptions.UnixFileMode_UserRead;

        if ((unixFileMode & UnixFileMode.StickyBit) != 0)
            value |= AllOptions.UnixFileMode_StickyBit;

        if ((unixFileMode & UnixFileMode.SetGroup) != 0)
            value |= AllOptions.UnixFileMode_SetGroup;

        if ((unixFileMode & UnixFileMode.SetUser) != 0)
            value |= AllOptions.UnixFileMode_SetUser;

        return value;
    }

    private static UnixFileMode? AllOptionsToUnixFileMode(AllOptions options)
    {
        if ((options & AllOptions.UnixFileMode_NotNull) == 0)
            return null;

        UnixFileMode value = 0;

        if ((options & AllOptions.UnixFileMode_OtherExecute) != 0)
            value |= UnixFileMode.OtherExecute;

        if ((options & AllOptions.UnixFileMode_OtherWrite) != 0)
            value |= UnixFileMode.OtherWrite;

        if ((options & AllOptions.UnixFileMode_OtherRead) != 0)
            value |= UnixFileMode.OtherRead;

        if ((options & AllOptions.UnixFileMode_GroupExecute) != 0)
            value |= UnixFileMode.GroupExecute;

        if ((options & AllOptions.UnixFileMode_GroupWrite) != 0)
            value |= UnixFileMode.GroupWrite;

        if ((options & AllOptions.UnixFileMode_GroupRead) != 0)
            value |= UnixFileMode.GroupRead;

        if ((options & AllOptions.UnixFileMode_UserExecute) != 0)
            value |= UnixFileMode.UserExecute;

        if ((options & AllOptions.UnixFileMode_UserWrite) != 0)
            value |= UnixFileMode.UserWrite;

        if ((options & AllOptions.UnixFileMode_UserRead) != 0)
            value |= UnixFileMode.UserRead;

        if ((options & AllOptions.UnixFileMode_StickyBit) != 0)
            value |= UnixFileMode.StickyBit;

        if ((options & AllOptions.UnixFileMode_SetGroup) != 0)
            value |= UnixFileMode.SetGroup;

        if ((options & AllOptions.UnixFileMode_SetUser) != 0)
            value |= UnixFileMode.SetUser;

        return value;
    }

    private const ulong ModeMask = 0b111;

    private enum AllOptions : ulong
    {
        Mode_CreateNew = 1,
        Mode_Create = 2,
        Mode_Open = 3,
        Mode_OpenOrCreate = 4,
        Mode_Truncate = 5,
        Mode_Append = 6,

        Access_ReadAttributes = 1 << 3,
        Access_WriteAttributes = 1 << 4,
        Access_Read = 1 << 5,
        Access_Write = 1 << 6,
        Access_Delete = 1 << 7,
        Access_Execute = 1 << 8,

        Share_Read = 1 << 9,
        Share_Write = 1 << 10,
        Share_Delete = 1 << 11,
        Share_Inheritable = 1 << 12,

        Options_WriteThrough = 1 << 13,
        Options_Asynchronous = 1 << 14,
        Options_RandomAccess = 1 << 15,
        Options_DeleteOnClose = 1 << 16,
        Options_SequentialScan = 1 << 17,
        Options_Encrypted = 1 << 18,

        Attributes_ReadOnly = 1 << 19,
        Attributes_Hidden = 1 << 20,
        Attributes_System = 1 << 21,
        Attributes_Directory = 1 << 22,
        Attributes_Archive = 1 << 23,
        Attributes_Device = 1 << 24,
        Attributes_Normal = 1 << 25,
        Attributes_Temporary = 1 << 26,
        Attributes_SparseFile = 1 << 27,
        Attributes_ReparsePoint = 1 << 28,
        Attributes_Compressed = 1 << 29,
        Attributes_Offline = 1 << 30,
        Attributes_NotContentIndexed = 1ul << 31,
        Attributes_Encrypted = 1ul << 32,
        Attributes_IntegrityStream = 1ul << 33,
        Attributes_NoScrubData = 1ul << 34,

        UnixFileMode_NotNull = 1ul << 35,
        UnixFileMode_OtherExecute = 1ul << 36,
        UnixFileMode_OtherWrite = 1ul << 37,
        UnixFileMode_OtherRead = 1ul << 38,
        UnixFileMode_GroupExecute = 1ul << 39,
        UnixFileMode_GroupWrite = 1ul << 40,
        UnixFileMode_GroupRead = 1ul << 41,
        UnixFileMode_UserExecute = 1ul << 42,
        UnixFileMode_UserWrite = 1ul << 43,
        UnixFileMode_UserRead = 1ul << 44,
        UnixFileMode_StickyBit = 1ul << 45,
        UnixFileMode_SetGroup = 1ul << 46,
        UnixFileMode_SetUser = 1ul << 47,
    }
}

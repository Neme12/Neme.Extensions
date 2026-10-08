using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FileHandleRequest
{
    private readonly AllOptions _allOptions;
    private readonly long _preallocationSize;

    public FileHandleRequest()
    {
        _allOptions = ToAllOptions(
            0,
            FileSystemAccess.None,
            FileShare.All,
            FileHandleType.RegularFile,
            FileOptions.None,
            0,
            null);
    }

    public FileHandleRequest(FileMode mode, FileSystemAccess access)
    {
        Require.ArgumentDefined(mode);
        Require.ArgumentFlagsDefined(access);

        _allOptions = ToAllOptions(
            mode,
            access,
            FileHandleOptions.GetDefaultFileShare(access),
            FileHandleType.RegularFile,
            FileOptions.None,
            0,
            null);
    }

    public FileHandleRequest(
        FileMode mode,
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None)
    {
        Require.ArgumentDefined(mode);
        Require.ArgumentFlagsDefined(access);
        Require.ArgumentFlagsDefined(share);
        Require.ArgumentDefined(type);
        Require.ArgumentFlagsDefined(flags);

        _allOptions = ToAllOptions(mode, access, share, type, flags, 0, null);
    }

    public FileHandleRequest(FileMode mode, FileHandleOptions handleOptions, FileCreationOptions creationOptions = default)
    {
        Require.ArgumentDefined(mode);

        _allOptions = ToAllOptions(
            mode,
            handleOptions.Access,
            handleOptions.Share,
            handleOptions.Type,
            handleOptions.Flags,
            creationOptions.Attributes,
            creationOptions.UnixCreateMode);
        _preallocationSize = creationOptions.PreallocationSize;
    }

    public void Deconstruct(out FileMode mode, out FileHandleOptions handleOptions)
    {
        mode = Mode;
        handleOptions = HandleOptions;
    }

    public void Deconstruct(out FileMode mode, out FileHandleOptions handleOptions, out FileCreationOptions creationOptions)
    {
        mode = Mode;
        handleOptions = HandleOptions;
        creationOptions = CreationOptions;
    }

    public static FileHandleRequest Create(FileSystemAccess access) =>
        new(FileMode.Create, access, FileHandleOptions.GetDefaultFileShare(access));

    public static FileHandleRequest Create(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None) =>
        new(FileMode.Create, access, share, type, flags);

    public static FileHandleRequest Create(FileHandleOptions handleOptions, FileCreationOptions creationOptions = default) =>
        new(FileMode.Create, handleOptions, creationOptions);

    public static FileHandleRequest CreateNew(FileSystemAccess access) =>
        new(FileMode.CreateNew, access, FileHandleOptions.GetDefaultFileShare(access));

    public static FileHandleRequest CreateNew(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None) =>
        new(FileMode.CreateNew, access, share, type, flags);

    public static FileHandleRequest CreateNew(FileHandleOptions handleOptions, FileCreationOptions creationOptions = default) =>
        new(FileMode.CreateNew, handleOptions, creationOptions);

    public static FileHandleRequest Open(FileSystemAccess access) =>
        new(FileMode.Open, access, FileHandleOptions.GetDefaultFileShare(access));

    public static FileHandleRequest Open(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None) =>
        new(FileMode.Open, access, share, type, flags);

    public static FileHandleRequest Open(FileHandleOptions handleOptions, FileCreationOptions creationOptions = default) =>
        new(FileMode.Open, handleOptions, creationOptions);

    public static FileHandleRequest OpenOrCreate(FileSystemAccess access) =>
        new(FileMode.OpenOrCreate, access, FileHandleOptions.GetDefaultFileShare(access));

    public static FileHandleRequest OpenOrCreate(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None) =>
        new(FileMode.OpenOrCreate, access, share, type, flags);

    public static FileHandleRequest OpenOrCreate(FileHandleOptions handleOptions, FileCreationOptions creationOptions = default) =>
        new(FileMode.OpenOrCreate, handleOptions, creationOptions);

    public static FileHandleRequest Truncate(FileSystemAccess access = FileSystemAccess.Write) =>
        new(FileMode.Truncate, access, FileHandleOptions.GetDefaultFileShare(access));

    public static FileHandleRequest Truncate(
        FileSystemAccess access = FileSystemAccess.Write,
        FileShare share = FileShare.None,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None) =>
        new(FileMode.Truncate, access, share, type, flags);

    public static FileHandleRequest Truncate(FileHandleOptions handleOptions, FileCreationOptions creationOptions = default) =>
        new(FileMode.Truncate, handleOptions, creationOptions);

    public static FileHandleRequest Append(FileSystemAccess access = FileSystemAccess.Write) =>
        new(FileMode.Append, access, FileHandleOptions.GetDefaultFileShare(access));

    public static FileHandleRequest Append(
        FileSystemAccess access = FileSystemAccess.Write,
        FileShare share = FileShare.None,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None) =>
        new(FileMode.Append, access, share, type, flags);

    public static FileHandleRequest Append(FileHandleOptions handleOptions, FileCreationOptions creationOptions = default) =>
        new(FileMode.Append, handleOptions, creationOptions);

    public FileMode Mode
    {
        get => AllOptionsToMode(_allOptions);
        init
        {
            Require.ArgumentDefined(value);
            _allOptions = ToAllOptions(value, Access, Share, Type, Flags, Attributes, UnixCreateMode);
        }
    }

    public FileSystemAccess Access
    {
        get => AllOptionsToAccess(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(Mode, value, Share, Type, Flags, Attributes, UnixCreateMode);
        }
    }

    public FileShare Share
    {
        get => AllOptionsToShare(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(Mode, Access, value, Type, Flags, Attributes, UnixCreateMode);
        }
    }

    public FileHandleType Type
    {
        get => AllOptionsToType(_allOptions);
        init
        {
            Require.ArgumentDefined(value);
            _allOptions = ToAllOptions(Mode, Access, Share, value, Flags, Attributes, UnixCreateMode);
        }
    }

    public FileOptions Flags
    {
        get => AllOptionsToFlags(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(Mode, Access, Share, Type, value, Attributes, UnixCreateMode);
        }
    }

    public FileAttributes Attributes
    {
        get => AllOptionsToAttributes(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(Mode, Access, Share, Type, Flags, value, UnixCreateMode);
        }
    }

    public UnixFileMode? UnixCreateMode
    {
        get => AllOptionsToUnixFileMode(_allOptions);
        [UnsupportedOSPlatform("windows")]
        init
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException(Strings.PlatformNotSupported_UnixFileMode);

            if (value is not null)
                Require.ArgumentFlagsDefined(value.Value);

            _allOptions = ToAllOptions(Mode, Access, Share, Type, Flags, Attributes, value);
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

    public FileHandleOptions HandleOptions
    {
        get => new(
            AllOptionsToAccess(_allOptions),
            AllOptionsToShare(_allOptions),
            AllOptionsToType(_allOptions),
            AllOptionsToFlags(_allOptions));
        init =>
            _allOptions = ToAllOptions(
                Mode,
                value.Access,
                value.Share,
                value.Type,
                value.Flags,
                Attributes,
                UnixCreateMode);
    }

    public FileCreationOptions CreationOptions
    {
        get => new(
            AllOptionsToAttributes(_allOptions),
            AllOptionsToUnixFileMode(_allOptions),
            PreallocationSize);
        init
        {
            _allOptions = ToAllOptions(
                Mode,
                Access,
                Share,
                Type,
                Flags,
                value.Attributes,
                value.UnixCreateMode);
            _preallocationSize = value.PreallocationSize;
        }
    }


#if NET6_0_OR_GREATER
    public static FileHandleRequest FromFileStreamOptions(FileStreamOptions options)
    {
        Require.ArgumentNotNull(options);

        var result = new FileHandleRequest
        {
            Mode = options.Mode,
            Access = FileSystemAccess.FromFileAccess(options.Access),
            Share = options.Share,
            Flags = options.Options,
        };

#if NET7_0_OR_GREATER
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            result = result with { UnixCreateMode = options.UnixCreateMode };
#endif

        return result;
    }

    public static implicit operator FileHandleRequest(FileStreamOptions options) =>
        FromFileStreamOptions(options);
#endif

    private static AllOptions ToAllOptions(
        FileMode mode,
        FileSystemAccess access,
        FileShare share,
        FileHandleType type,
        FileOptions flags,
        FileAttributes attributes,
        UnixFileMode? unixFileMode)
    {
        return
            ModeToAllOptions(mode) |
            AccessToAllOptions(access) |
            ShareToAllOptions(share) |
            TypeToAllOptions(type) |
            FlagsToAllOptions(flags) |
            AttributesToAllOptions(attributes) |
            UnixFileModeToAllOptions(unixFileMode);
    }

    private static AllOptions ModeToAllOptions(FileMode mode)
    {
        return (AllOptions)((ulong)mode & ModeMask);
    }

    private static FileMode AllOptionsToMode(AllOptions options)
    {
        return (FileMode)((ulong)options & ModeMask);
    }

    private static AllOptions TypeToAllOptions(FileHandleType type)
    {
        return (AllOptions)(((ulong)type & TypeMask) << TypeShift);
    }

    private static FileHandleType AllOptionsToType(AllOptions options)
    {
        return (FileHandleType)(((ulong)options >> TypeShift) & TypeMask);
    }

    private static AllOptions AccessToAllOptions(FileSystemAccess access)
    {
        AllOptions value = 0;

        var rawAccess = (RawFileSystemAccess)access;

        if ((rawAccess & RawFileSystemAccess.ReadAttributes) != 0)
            value |= AllOptions.Access_ReadAttributes;

        if ((rawAccess & RawFileSystemAccess.WriteAttributes) != 0)
            value |= AllOptions.Access_WriteAttributes;

        if ((rawAccess & RawFileSystemAccess.Read) != 0)
            value |= AllOptions.Access_Read;

        if ((rawAccess & RawFileSystemAccess.Write) != 0)
            value |= AllOptions.Access_Write;

        if ((rawAccess & RawFileSystemAccess.Delete) != 0)
            value |= AllOptions.Access_Delete;

        if ((rawAccess & RawFileSystemAccess.Execute) != 0)
            value |= AllOptions.Access_Execute;

        return value;
    }

    private static FileSystemAccess AllOptionsToAccess(AllOptions options)
    {
        RawFileSystemAccess value = 0;

        if ((options & AllOptions.Access_ReadAttributes) != 0)
            value |= RawFileSystemAccess.ReadAttributes;

        if ((options & AllOptions.Access_WriteAttributes) != 0)
            value |= RawFileSystemAccess.WriteAttributes;

        if ((options & AllOptions.Access_Read) != 0)
            value |= RawFileSystemAccess.Read;

        if ((options & AllOptions.Access_Write) != 0)
            value |= RawFileSystemAccess.Write;

        if ((options & AllOptions.Access_Delete) != 0)
            value |= RawFileSystemAccess.Delete;

        if ((options & AllOptions.Access_Execute) != 0)
            value |= RawFileSystemAccess.Execute;

        return (FileSystemAccess)value;
    }

    private static AllOptions ShareToAllOptions(FileShare share)
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

    private static FileShare AllOptionsToShare(AllOptions options)
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

    private static AllOptions FlagsToAllOptions(FileOptions flags)
    {
        AllOptions value = 0;

        if ((flags & FileOptions.WriteThrough) != 0)
            value |= AllOptions.Flags_WriteThrough;

        if ((flags & FileOptions.Asynchronous) != 0)
            value |= AllOptions.Flags_Asynchronous;

        if ((flags & FileOptions.NoBuffering) != 0)
            value |= AllOptions.Flags_NoBuffering;

        if ((flags & FileOptions.RandomAccess) != 0)
            value |= AllOptions.Flags_RandomAccess;

        if ((flags & FileOptions.DeleteOnClose) != 0)
            value |= AllOptions.Flags_DeleteOnClose;

        if ((flags & FileOptions.SequentialScan) != 0)
            value |= AllOptions.Flags_SequentialScan;

        if ((flags & FileOptions.AllowPosix) != 0)
            value |= AllOptions.Flags_AllowPosix;

        if ((flags & FileOptions.BackupOrRestore) != 0)
            value |= AllOptions.Flags_BackupOrRestore;

        if ((flags & FileOptions.DisallowReparsePoint) != 0)
            value |= AllOptions.Flags_DisallowReparsePoint;

        if ((flags & FileOptions.NoRemoteRecall) != 0)
            value |= AllOptions.Flags_NoRemoteRecall;

        if ((flags & FileOptions.FirstPipeInstance) != 0)
            value |= AllOptions.Flags_FirstPipeInstance;

        if ((flags & FileOptions.Encrypted) != 0)
            value |= AllOptions.Flags_Encrypted;

        return value;
    }

    private static FileOptions AllOptionsToFlags(AllOptions options)
    {
        FileOptions value = 0;

        if ((options & AllOptions.Flags_WriteThrough) != 0)
            value |= FileOptions.WriteThrough;

        if ((options & AllOptions.Flags_Asynchronous) != 0)
            value |= FileOptions.Asynchronous;

        if ((options & AllOptions.Flags_NoBuffering) != 0)
            value |= FileOptions.NoBuffering;

        if ((options & AllOptions.Flags_RandomAccess) != 0)
            value |= FileOptions.RandomAccess;

        if ((options & AllOptions.Flags_DeleteOnClose) != 0)
            value |= FileOptions.DeleteOnClose;

        if ((options & AllOptions.Flags_SequentialScan) != 0)
            value |= FileOptions.SequentialScan;

        if ((options & AllOptions.Flags_AllowPosix) != 0)
            value |= FileOptions.AllowPosix;

        if ((options & AllOptions.Flags_BackupOrRestore) != 0)
            value |= FileOptions.BackupOrRestore;

        if ((options & AllOptions.Flags_DisallowReparsePoint) != 0)
            value |= FileOptions.DisallowReparsePoint;

        if ((options & AllOptions.Flags_NoRemoteRecall) != 0)
            value |= FileOptions.NoRemoteRecall;

        if ((options & AllOptions.Flags_FirstPipeInstance) != 0)
            value |= FileOptions.FirstPipeInstance;

        if ((options & AllOptions.Flags_Encrypted) != 0)
            value |= FileOptions.Encrypted;

        return value;
    }

    private static AllOptions AttributesToAllOptions(FileAttributes attributes)
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

    private static FileAttributes AllOptionsToAttributes(AllOptions options)
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
    private const ulong TypeMask = 0b111;
    private const int TypeShift = 3;

    [Flags]
    private enum AllOptions : ulong
    {
        Mode_CreateNew = FileMode.CreateNew,
        Mode_Create = FileMode.Create,
        Mode_Open = FileMode.Open,
        Mode_OpenOrCreate = FileMode.OpenOrCreate,
        Mode_Truncate = FileMode.Truncate,
        Mode_Append = FileMode.Append,

        // Type_Unknown = 0
        Type_RegularFile = FileHandleType.RegularFile << TypeShift,
        Type_Pipe = FileHandleType.Pipe << TypeShift,
        Type_Socket = FileHandleType.Socket << TypeShift,
        Type_CharacterDevice = FileHandleType.CharacterDevice << TypeShift,
        Type_Directory = FileHandleType.Directory << TypeShift,
        Type_SymbolicLink = FileHandleType.SymbolicLink << TypeShift,
        Type_BlockDevice = FileHandleType.BlockDevice << TypeShift,

        // Access_None = 0
        Access_ReadAttributes = 1 << 6,
        Access_WriteAttributes = 1 << 7,
        Access_Read = 1 << 8,
        Access_Write = 1 << 9,
        Access_Delete = 1 << 10,
        Access_Execute = 1 << 11,

        // Share_None = 0
        Share_Read = 1 << 12,
        Share_Write = 1 << 13,
        Share_Delete = 1 << 14,
        Share_Inheritable = 1 << 15,

        // Flags_None = 0
        Flags_WriteThrough = 1 << 16,
        Flags_Asynchronous = 1 << 17,
        Flags_NoBuffering = 1 << 18,
        Flags_RandomAccess = 1 << 19,
        Flags_DeleteOnClose = 1 << 20,
        Flags_SequentialScan = 1 << 21,
        Flags_AllowPosix = 1 << 22,
        Flags_BackupOrRestore = 1 << 23,
        Flags_DisallowReparsePoint = 1 << 24,
        Flags_NoRemoteRecall = 1 << 25,
        Flags_FirstPipeInstance = 1 << 26,
        Flags_Encrypted = 1 << 27,

        // Attributes_None = 0
        Attributes_ReadOnly = 1 << 28,
        Attributes_Hidden = 1 << 29,
        Attributes_System = 1 << 30,
        Attributes_Directory = 1ul << 31,
        Attributes_Archive = 1ul << 32,
        Attributes_Device = 1ul << 33,
        Attributes_Normal = 1ul << 34,
        Attributes_Temporary = 1ul << 35,
        Attributes_SparseFile = 1ul << 36,
        Attributes_ReparsePoint = 1ul << 37,
        Attributes_Compressed = 1ul << 38,
        Attributes_Offline = 1ul << 39,
        Attributes_NotContentIndexed = 1ul << 40,
        Attributes_Encrypted = 1ul << 41,
        Attributes_IntegrityStream = 1ul << 42,
        Attributes_NoScrubData = 1ul << 43,

        // UnixFileMode_None = 0
        UnixFileMode_NotNull = 1ul << 44,
        UnixFileMode_OtherExecute = 1ul << 45,
        UnixFileMode_OtherWrite = 1ul << 46,
        UnixFileMode_OtherRead = 1ul << 47,
        UnixFileMode_GroupExecute = 1ul << 48,
        UnixFileMode_GroupWrite = 1ul << 49,
        UnixFileMode_GroupRead = 1ul << 50,
        UnixFileMode_UserExecute = 1ul << 51,
        UnixFileMode_UserWrite = 1ul << 52,
        UnixFileMode_UserRead = 1ul << 53,
        UnixFileMode_StickyBit = 1ul << 54,
        UnixFileMode_SetGroup = 1ul << 55,
        UnixFileMode_SetUser = 1ul << 56,
    }
}

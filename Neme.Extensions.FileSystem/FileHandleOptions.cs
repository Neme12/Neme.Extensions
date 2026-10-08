using Neme.Extensions.Contracts;
using Neme.Extensions.IO;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Auto)]
public readonly record struct FileHandleOptions
{
    private readonly AllOptions _allOptions;

    public static FileCreationOptions None =>
        default;

    public FileHandleOptions()
    {
        _allOptions = ToAllOptions(
            FileSystemAccess.None,
            FileShare.All,
            FileHandleType.RegularFile,
            FileOptions.None);
    }

    public FileHandleOptions(
        FileSystemAccess access)
    {
        Require.ArgumentFlagsDefined(access);

        _allOptions = ToAllOptions(
            access,
            GetDefaultFileShare(access),
            FileHandleType.RegularFile,
            FileOptions.None);
    }

    public FileHandleOptions(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.None)
    {
        Require.ArgumentFlagsDefined(access);
        Require.ArgumentFlagsDefined(share);
        Require.ArgumentDefined(type);
        Require.ArgumentFlagsDefined(flags);

        _allOptions = ToAllOptions(access, share, type, flags);
    }

    public void Deconstruct(out FileSystemAccess access)
    {
        access = Access;
    }

    public void Deconstruct(out FileSystemAccess access, out FileShare share)
    {
        access = Access;
        share = Share;
    }

    public void Deconstruct(out FileSystemAccess access, out FileShare share, out FileHandleType type, out FileOptions flags)
    {
        access = Access;
        share = Share;
        type = Type;
        flags = Flags;
    }

    internal static FileShare GetDefaultFileShare(FileSystemAccess access)
    {
        var rawAccess = (RawFileSystemAccess)access;

        if ((rawAccess & RawFileSystemAccess.Write) != 0 ||
            (rawAccess & RawFileSystemAccess.Delete) != 0)
            return FileShare.None;

        if ((rawAccess & RawFileSystemAccess.Read) != 0 ||
            (rawAccess & RawFileSystemAccess.Execute) != 0)
            return FileShare.Read;

        return FileShare.ReadWrite | FileShare.Delete;
    }

    public FileSystemAccess Access
    {
        get => AllOptionsToAccess(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(value, Share, Type, Flags);
        }
    }

    public FileShare Share
    {
        get => AllOptionsToShare(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(Access, value, Type, Flags);
        }
    }

    public FileHandleType Type
    {
        get => AllOptionsToType(_allOptions);
        init
        {
            Require.ArgumentDefined(value);
            _allOptions = ToAllOptions(Access, Share, value, Flags);
        }
    }

    public FileOptions Flags
    {
        get => AllOptionsToFlags(_allOptions);
        init
        {
            Require.ArgumentFlagsDefined(value);
            _allOptions = ToAllOptions(Access, Share, Type, value);
        }
    }

    private static AllOptions ToAllOptions(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type,
        FileOptions flags)
    {
        return
            AccessToAllOptions(access) |
            ShareToAllOptions(share) |
            TypeToAllOptions(type) |
            FlagsToAllOptions(flags);
    }

    private static AllOptions TypeToAllOptions(FileHandleType type)
    {
        return (AllOptions)((uint)type & TypeMask);
    }

    private static FileHandleType AllOptionsToType(AllOptions options)
    {
        return (FileHandleType)((uint)options & TypeMask);
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

#if NET6_0_OR_GREATER
    public static FileHandleOptions FromFileStreamOptions(FileStreamOptions options)
    {
        Require.ArgumentNotNull(options);

        return new FileHandleOptions
        {
            Access = FileSystemAccess.FromFileAccess(options.Access),
            Share = options.Share,
            Flags = options.Options,
        };
    }

    public static implicit operator FileHandleOptions(FileStreamOptions options) =>
        FromFileStreamOptions(options);
#endif

    private const ulong TypeMask = 0b111;

    [Flags]
    private enum AllOptions : uint
    {
        // Type_Unknown = 0,
        Type_RegularFile = FileHandleType.RegularFile,
        Type_Pipe = FileHandleType.Pipe,
        Type_Socket = FileHandleType.Socket,
        Type_CharacterDevice = FileHandleType.CharacterDevice,
        Type_Directory = FileHandleType.Directory,
        Type_SymbolicLink = FileHandleType.SymbolicLink,
        Type_BlockDevice = FileHandleType.BlockDevice,

        // Access_None = 0
        Access_ReadAttributes = 1 << 3,
        Access_WriteAttributes = 1 << 4,
        Access_Read = 1 << 5,
        Access_Write = 1 << 6,
        Access_Delete = 1 << 7,
        Access_Execute = 1 << 8,

        // Share_None = 0
        Share_Read = 1 << 9,
        Share_Write = 1 << 10,
        Share_Delete = 1 << 11,
        Share_Inheritable = 1 << 12,

        // Flags_None = 0
        Flags_WriteThrough = 1 << 13,
        Flags_Asynchronous = 1 << 14,
        Flags_NoBuffering = 1 << 15,
        Flags_RandomAccess = 1 << 16,
        Flags_DeleteOnClose = 1 << 17,
        Flags_SequentialScan = 1 << 18,
        Flags_AllowPosix = 1 << 19, 
        Flags_BackupOrRestore = 1 << 20,
        Flags_DisallowReparsePoint = 1 << 21,
        Flags_NoRemoteRecall = 1 << 22,
        Flags_FirstPipeInstance = 1 << 23,
        Flags_Encrypted = 1 << 24,
    }
}

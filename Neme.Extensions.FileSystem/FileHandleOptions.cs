using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileHandleOptions
{
    private readonly AllOptions _allOptions;

    public FileHandleOptions(
        FileSystemAccess access,
        FileShare share,
        FileOptions flags = FileOptions.None)
    {
        _allOptions = ToAllOptions(access, share, flags);
    }

    public FileSystemAccess Access
    {
        get => AllOptionsToFileSystemAccess(_allOptions);
        init => _allOptions = ToAllOptions(value, Share, Flags);
    }

    public FileShare Share
    {
        get => AllOptionsToFileShare(_allOptions);
        init => _allOptions = ToAllOptions(Access, value, Flags);
    }

    public FileOptions Flags
    {
        get => AllOptionsToFileOptions(_allOptions);
        init => _allOptions = ToAllOptions(Access, Share, value);
    }

    private static AllOptions ToAllOptions(
        FileSystemAccess access,
        FileShare share,
        FileOptions flags)
    {
        return
            FileSystemAccessToAllOptions(access) |
            FileShareToAllOptions(share) |
            FileOptionsToAllOptions(flags);
    }

    private static AllOptions FileSystemAccessToAllOptions(FileSystemAccess access)
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

    private static FileSystemAccess AllOptionsToFileSystemAccess(AllOptions options)
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

    private static AllOptions FileOptionsToAllOptions(FileOptions flags)
    {
        AllOptions value = 0;

        if ((flags & FileOptions.WriteThrough) != 0)
            value |= AllOptions.Options_WriteThrough;

        if ((flags & FileOptions.Asynchronous) != 0)
            value |= AllOptions.Options_Asynchronous;

        if ((flags & FileOptions.RandomAccess) != 0)
            value |= AllOptions.Options_RandomAccess;

        if ((flags & FileOptions.DeleteOnClose) != 0)
            value |= AllOptions.Options_DeleteOnClose;

        if ((flags & FileOptions.SequentialScan) != 0)
            value |= AllOptions.Options_SequentialScan;

        if ((flags & FileOptions.Encrypted) != 0)
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

#if NET6_0_OR_GREATER
    public static FileHandleOptions FromFileStreamOptions(FileStreamOptions options)
    {
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

    private enum AllOptions : uint
    {
        Access_ReadAttributes = 1 << 1,
        Access_WriteAttributes = 1 << 2,
        Access_Read = 1 << 3,
        Access_Write = 1 << 4,
        Access_Delete = 1 << 5,
        Access_Execute = 1 << 6,

        Share_Read = 1 << 7,
        Share_Write = 1 << 8,
        Share_Delete = 1 << 9,
        Share_Inheritable = 1 << 10,

        Options_WriteThrough = 1 << 11,
        Options_Asynchronous = 1 << 12,
        Options_RandomAccess = 1 << 13,
        Options_DeleteOnClose = 1 << 14,
        Options_SequentialScan = 1 << 15,
        Options_Encrypted = 1 << 16,
    }
}

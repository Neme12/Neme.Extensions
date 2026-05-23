using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Ownership;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public static partial class FileIO
{
    private static FileIOStrategy? _strategyLazy;

#pragma warning disable CA1416 // Old Windows versions are not supported
#pragma warning disable RS0042
    private static FileIOStrategy Strategy => LazyInitializer.EnsureInitialized(ref _strategyLazy, () =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WindowsFileIOStrategy()
            : throw new PlatformNotSupportedException("FileIO is only supported on Windows."))!;
#pragma warning restore RS0042
#pragma warning restore CA1416

    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandle(string path, FsFileOptions options) =>
        Strategy.OpenHandle(path, options);

    [return: OwnershipTransfer]
    public static FsFile Open(string path, FsFileOptions options) =>
        new(OpenHandle(path, options), options);

    public static bool TryOpenHandle(
        string path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
        bool requireDirectory = true)
    {
        try
        {
            handle = OpenHandle(path, options);
            return true;

        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            handle = null;
            return false;
        }
    }

    public static bool TryOpen(
        string path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        try
        {
            file = Open(path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [return: OwnershipTransfer]
    public static CustomSafeFileHandle OpenHandle(
        FsFileId fileId,
        FsFileOptions options)
    {
        return Strategy.OpenHandle(fileId, options);
    }

    [return: OwnershipTransfer]
    public static CustomSafeFileHandle Open(
        FsFileId fileId,
        FsFileOptions options)
    {
        throw new NotImplementedException();
    }

    public static bool TryOpenHandle(
        FsFileId fileId,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out CustomSafeFileHandle? handle,
        bool requireDirectory = true)
    {
        try
        {
            handle = OpenHandle(fileId, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            handle = null;
            return false;
        }
    }

    public static bool TryOpen(
        FsFileId fileId,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        throw new NotImplementedException();
        //try
        //{
        //    file = Open(fileId, options);
        //    return true;
        //}
        //catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        //{
        //    file = null;
        //    return false;
        //}
    }

    [return: OwnershipTransfer]
    public static CustomSafeFileHandle OpenHandleBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
    {
        return Strategy.OpenHandleBy(rootDirectory, path, options);
    }

    [return: OwnershipTransfer]
    public static FsFile OpenBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
    {
        throw new NotImplementedException();
        //return new(OpenHandleBy(rootDirectory, path, options), options);
    }

    public static bool TryOpenHandleBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out CustomSafeFileHandle? file,
        bool requireDirectory = true)
    {
        try
        {
            file = OpenHandleBy(rootDirectory, path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    public static bool TryOpenBy(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        try
        {
            file = OpenBy(rootDirectory, path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [return: OwnershipTransfer]
    public static CustomSafeFileHandle ReopenHandle([Borrow] SafeFileHandle file, FsFileOptions options)
    {
        FileIOStrategy.ValidateFileHandle(file);

        return OpenHandleBy(file, null, options);
    }

    [return: OwnershipTransfer]
    public static FsFile Reopen([Borrow] FsFile file, FsFileOptions? options = null)
    {
        throw new NotImplementedException();
        //FileIOStrategy.ValidateFileHandle(file.Handle);

        //return new(OpenHandleBy(file.Handle, null, file.Options), options ?? file.Options);
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FsFileAccess? access) =>
        Strategy.DuplicateHandle(file, access);

    [return: OwnershipTransfer]
    public static FsFile Duplicate([Borrow] FsFile file) =>
        new(DuplicateHandle(file.Handle, file.Options.Access), file.Options);

    public static string GetPath([Borrow] SafeFileHandle file) =>
        Strategy.GetPath(file);

    public static string GetPath(FsFileId fileId) =>
        Strategy.GetPath(fileId);

    public static void Move([Borrow] SafeFileHandle sourceFile, string destFileName, bool overwrite = false) =>
        Strategy.Move(sourceFile, destFileName, overwrite);

    public static void Delete([Borrow] SafeFileHandle file) =>
        Strategy.Delete(file);

    public static void SetFileAttributes([Borrow] SafeFileHandle file, FileAttributes attributes) =>
        Strategy.SetFileAttributes(file, attributes);

    public static FileAttributes GetFileAttributes([Borrow] SafeFileHandle file) =>
        Strategy.GetFileAttributes(file);

    public static FsFileInfo GetFileInfo([Borrow] SafeFileHandle file) =>
        Strategy.GetFileInfo(file);

    public static FsFileId GetFileId([Borrow] SafeHandle file) =>
        Strategy.GetFileId(file);

    [return: OwnershipTransfer]
    public static CheckedFileStream CreateFileStream(
        [OwnershipTransfer] SafeFileHandle file,
        FsFileOptions options,
        bool leaveOpen = false,
        int bufferSize = 4096)
    {
        FileIOStrategy.ValidateFileHandle(file);

        using var handle = OwnedOrBorrowed.Create(leaveOpen
            ? new SafeFileHandle(file.DangerousGetHandle(), ownsHandle: false)
            : file);

        FileAccess access = 0;

        if ((options.Access & FsFileAccess.Read) != 0)
            access |= FileAccess.Read;

        if ((options.Access & FsFileAccess.Write) != 0)
            access |= FileAccess.Write;

        return new CheckedFileStream(handle.Move(), access, bufferSize, isAsync: (options.Options & FileOptions.Asynchronous) != 0);
    }
}

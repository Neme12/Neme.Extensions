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
#if NETFRAMEWORK
        new WindowsFileIOStrategy())!;
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WindowsFileIOStrategy()
            : new UnixFileIOStrategy())!;
#endif
#pragma warning restore RS0042
#pragma warning restore CA1416

    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandle(string path, FsFileOptions options)
    {
        Strategy.ValidatePath(path);

        return Strategy.OpenHandle(path, options);
    }

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
    public static SafeFileHandle OpenHandle(
        FsFileId fileId,
        FsFileOptions options)
    {
        Strategy.ValidateFileId(fileId);

        return Strategy.OpenHandle(fileId, options);
    }

    [return: OwnershipTransfer]
    public static FsFile Open(
        FsFileId fileId,
        FsFileOptions options)
    {
        return new(OpenHandle(fileId, options), options);
    }

    public static bool TryOpenHandle(
        FsFileId fileId,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
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
        try
        {
            file = Open(fileId, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandleAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
    {
        if (rootDirectory is null && path is null)
            throw new ArgumentException($"Either {nameof(rootDirectory)} or {nameof(path)} must be provided.");

        Strategy.ValidateFileHandle(rootDirectory, optional: true);
        Strategy.ValidatePath(path, optional: true);

        return Strategy.OpenHandleAt(rootDirectory, path, options);
    }

    [return: OwnershipTransfer]
    public static FsFile OpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options)
    {
        return new(OpenHandleAt(rootDirectory, path, options), options);
    }

    public static bool TryOpenHandleAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? file,
        bool requireDirectory = true)
    {
        try
        {
            file = OpenHandleAt(rootDirectory, path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    public static bool TryOpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FsFileOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out FsFile? file,
        bool requireDirectory = true)
    {
        try
        {
            file = OpenAt(rootDirectory, path, options);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || !requireDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle ReopenHandle([Borrow] SafeFileHandle file, FsFileOptions options)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.OpenHandleAt(file, null, options);
    }

    [return: OwnershipTransfer]
    public static FsFile Reopen([Borrow] FsFile file, FsFileOptions? options = null) =>
        new(OpenHandleAt(file.Handle, null, file.Options), options ?? file.Options);

    [return: OwnershipTransfer]
    public static SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FsFileAccess? access)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.DuplicateHandle(file, access);
    }

    [return: OwnershipTransfer]
    public static FsFile Duplicate([Borrow] FsFile file) =>
        new(DuplicateHandle(file.Handle, file.Options.Access), file.Options);

    public static string GetPath([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetPath(file);
    }

    public static string GetPath(FsFileId fileId)
    {
        Strategy.ValidateFileId(fileId);

        var options = new FsFileOptions(FileMode.Open, FsFileAccess.ReadAttributes, FileShare.ReadWrite | FileShare.Delete);
        using (var handle = Strategy.OpenHandle(fileId, options))
            return Strategy.GetPath(handle);
    }

    public static void Move([Borrow] SafeFileHandle sourceFile, string destFileName, bool overwrite = false)
    {
        Strategy.ValidateFileHandle(sourceFile);
        Strategy.ValidatePath(destFileName);

        Strategy.Move(sourceFile, destFileName, overwrite);
    }

    public static void Delete([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        Strategy.Delete(file);
    }

    public static void SetFileAttributes([Borrow] SafeFileHandle file, FileAttributes attributes)
    {
        Strategy.ValidateFileHandle(file);

        Strategy.SetFileAttributes(file, attributes);
    }

    public static FileAttributes GetFileAttributes([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetFileAttributes(file);
    }

    public static FsFileInfo GetFileInfo([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetFileInfo(file);
    }

    public static FsFileId GetFileId([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetFileId(file);
    }

    [return: OwnershipTransferUnless(nameof(leaveOpen))]
    public static CheckedFileStream CreateFileStream(
        [OwnershipTransferUnless(nameof(leaveOpen))] SafeFileHandle file,
        FsFileOptions options,
        bool leaveOpen = false,
        int bufferSize = 4096)
    {
        Strategy.ValidateFileHandle(file);

        using var handle = OwnedOrBorrowed.Create(leaveOpen
            ? new SafeFileHandle(file.DangerousGetHandle(), ownsHandle: false)
            : file);

        return new CheckedFileStream(
            handle.Move(),
            options.Access.ToFileAccess(),
            bufferSize,
            isAsync: (options.Options & FileOptions.Asynchronous) != 0);
    }
}

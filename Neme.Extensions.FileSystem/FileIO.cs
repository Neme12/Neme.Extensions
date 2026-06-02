using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.FileIOStrategies;
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
    public static SafeFileHandle OpenHandle(string path, FileOpenOptions options)
    {
        Strategy.ValidatePath(path);

        return Strategy.OpenHandle(path, options);
    }

    [return: OwnershipTransfer]
    public static OpenFile Open(string path, FileOpenOptions options) =>
        new(OpenHandle(path, options), options);

    public static bool TryOpenHandle(
        string path,
        FileOpenOptions options,
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
        FileOpenOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out OpenFile? file,
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
        PersistentFileId fileId,
        FileOpenOptions options)
    {
        Strategy.ValidateFileId(fileId);

        return Strategy.OpenHandle(fileId, options);
    }

    [return: OwnershipTransfer]
    public static OpenFile Open(
        PersistentFileId fileId,
        FileOpenOptions options)
    {
        return new(OpenHandle(fileId, options), options);
    }

    public static bool TryOpenHandle(
        PersistentFileId fileId,
        FileOpenOptions options,
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
        PersistentFileId fileId,
        FileOpenOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out OpenFile? file,
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
        FileOpenOptions options)
    {
        if (rootDirectory is null && path is null)
            throw new ArgumentException($"Either {nameof(rootDirectory)} or {nameof(path)} must be provided.");

        Strategy.ValidateFileHandle(rootDirectory, optional: true);
        Strategy.ValidatePath(path, optional: true);

        return Strategy.OpenHandleAt(rootDirectory, path, options);
    }

    [return: OwnershipTransfer]
    public static OpenFile OpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileOpenOptions options)
    {
        return new(OpenHandleAt(rootDirectory, path, options), options);
    }

    public static bool TryOpenHandleAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileOpenOptions options,
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
        FileOpenOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out OpenFile? file,
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
    public static SafeFileHandle ReopenHandle([Borrow] SafeFileHandle file, FileOpenOptions options)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.OpenHandleAt(file, null, options);
    }

    [return: OwnershipTransfer]
    public static OpenFile Reopen([Borrow] OpenFile file, FileOpenOptions? options = null) =>
        new(OpenHandleAt(file.Handle, null, file.Options), options ?? file.Options);

    [return: OwnershipTransfer]
    public static SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FileSystemAccess? access)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.DuplicateHandle(file, access);
    }

    [return: OwnershipTransfer]
    public static OpenFile Duplicate([Borrow] OpenFile file) =>
        new(DuplicateHandle(file.Handle, file.Options.Access), file.Options);

    public static string GetPath([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetPath(file);
    }

    public static string GetPath(PersistentFileId fileId)
    {
        Strategy.ValidateFileId(fileId);

        var options = new FileOpenOptions(FileMode.Open, FileSystemAccess.ReadAttributes, FileShare.ReadWrite | FileShare.Delete);
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

    public static FileBasicInfo GetFileInfo([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetFileInfo(file);
    }

    public static PersistentFileId GetFileId([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetFileId(file);
    }

    [return: OwnershipTransferUnless(nameof(leaveOpen))]
    public static CheckedFileStream CreateFileStream(
        [OwnershipTransferUnless(nameof(leaveOpen))] SafeFileHandle file,
        FileOpenOptions options,
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

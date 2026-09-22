using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.FileIOStrategies;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.Ownership;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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

    public static bool TryOpenHandle(
        string path,
        FileOpenOptions options,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
        bool ignoreMissingDirectory = false)
    {
        try
        {
            handle = OpenHandle(path, options);
            return true;

        }
        catch (Exception e) when (e is FileNotFoundException || ignoreMissingDirectory && e is DirectoryNotFoundException)
        {
            handle = null;
            return false;
        }
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandle(
        PersistentFileId fileId,
        FileOpenOptions options)
    {
        Strategy.ValidateFileId(fileId);

        return Strategy.OpenHandle(fileId, options);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
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

    [return: OwnershipTransfer]
    public static SafeFileHandle ReopenHandle([Borrow] SafeFileHandle file, FileOpenOptions options)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.OpenHandleAt(file, null, options);
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle DuplicateHandle([Borrow] SafeFileHandle file, FileSystemAccess? access = null)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.DuplicateHandle(file, access);
    }

    public static string GetPath([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetPath(file);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
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

    public static void SetAttributes([Borrow] SafeFileHandle file, FileAttributes attributes)
    {
        Strategy.ValidateFileHandle(file);

        Strategy.SetAttributes(file, attributes);
    }

    public static FileAttributes GetAttributes([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetAttributes(file);
    }

    public static FileBasicInfo GetBasicInfo([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetBasicInfo(file);
    }

    public static FileId GetId([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetId(file);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static PersistentFileId GetPersistentId([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetPersistentId(file);
    }

    [return: OwnershipTransferWhen(nameof(ownsHandle))]
    public static CheckedFileStream CreateFileStream(
        [OwnershipTransferWhen(nameof(ownsHandle))] SafeFileHandle file,
        FileAccess access,
        bool ownsHandle = false,
        int bufferSize = 4096)
    {
        Strategy.ValidateFileHandle(file);

        return ownsHandle
            ? new CheckedFileStream(
                file,
                access,
                bufferSize,
                isAsync: file.IsAsync)
            : new LeaveOpenFileStream(
                file,
                access,
                bufferSize,
                isAsync: file.IsAsync);
    }
}

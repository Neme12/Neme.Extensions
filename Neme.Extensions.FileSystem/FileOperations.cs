using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.FileOperationsStrategies;
using Neme.Extensions.Ownership;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public static partial class FileOperations
{
    private static FileOperationsStrategy? _strategyLazy;

#pragma warning disable CA1416 // Old Windows versions are not supported
#pragma warning disable RS0042
    private static FileOperationsStrategy Strategy => LazyInitializer.EnsureInitialized(ref _strategyLazy, () =>
#if NETFRAMEWORK
        new WindowsFileOperationsStrategy())!;
#else
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new WindowsFileOperationsStrategy()
            : new UnixFileOperationsStrategy())!;
#endif
#pragma warning restore RS0042
#pragma warning restore CA1416

    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandle(string path, FileHandleRequest request)
    {
        Strategy.ValidatePath(path);

        return Strategy.OpenHandle(path, request);
    }

    public static bool TryOpenHandle(
        string path,
        FileHandleRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
        bool ignoreMissingDirectory = false)
    {
        try
        {
            handle = OpenHandle(path, request);
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
        FileHandleRequest request)
    {
        Strategy.ValidateFileId(fileId);

        return Strategy.OpenHandle(fileId, request);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryOpenHandle(
        PersistentFileId fileId,
        FileHandleRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? handle,
        bool ignoreMissingDirectory = false)
    {
        try
        {
            handle = OpenHandle(fileId, request);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || ignoreMissingDirectory && e is DirectoryNotFoundException)
        {
            handle = null;
            return false;
        }
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle OpenHandleAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileHandleRequest request)
    {
        if (rootDirectory is null && path is null)
            throw new ArgumentException($"Either {nameof(rootDirectory)} or {nameof(path)} must be provided.");

        Strategy.ValidateFileHandle(rootDirectory, optional: true);
        Strategy.ValidatePath(path, optional: true);

        return Strategy.OpenHandleAt(rootDirectory, path, request);
    }

    public static bool TryOpenHandleAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileHandleRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out SafeFileHandle? file,
        bool ignoreMissingDirectory = false)
    {
        try
        {
            file = OpenHandleAt(rootDirectory, path, request);
            return true;
        }
        catch (Exception e) when (e is FileNotFoundException || ignoreMissingDirectory && e is DirectoryNotFoundException)
        {
            file = null;
            return false;
        }
    }

    [return: OwnershipTransfer]
    public static SafeFileHandle ReopenHandle([Borrow] SafeFileHandle file, FileHandleRequest request)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.OpenHandleAt(file, null, request);
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

    public static FileAccess GetAccess([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetAccess(file);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static string GetPath(PersistentFileId fileId)
    {
        Strategy.ValidateFileId(fileId);

        var request = new FileHandleRequest(FileMode.Open, FileSystemAccess.ReadAttributes, FileShare.ReadWrite | FileShare.Delete);
        using (var handle = Strategy.OpenHandle(fileId, request))
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

    public static long Seek([Borrow] SafeFileHandle file, long offset, SeekOrigin origin)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.Seek(file, offset, origin);
    }

    public static long GetLength([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.GetLength(file);
    }

    public static void SetLength([Borrow] SafeFileHandle file, long length)
    {
        Strategy.ValidateFileHandle(file);
        Strategy.ValidateLength(length);

        Strategy.SetLength(file, length);
    }

    public static bool CanSeek([Borrow] SafeFileHandle file)
    {
        Strategy.ValidateFileHandle(file);

        return Strategy.CanSeek(file);
    }
}

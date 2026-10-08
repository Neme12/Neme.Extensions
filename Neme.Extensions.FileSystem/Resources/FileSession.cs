using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Internal;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using Neme.Extensions.SafeHandles;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Resources;

public sealed class FileSession : IFileResource, IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly FileHandleOptions _options;

    internal FileSession([OwnershipTransfer] SafeFileHandle handle, FileHandleOptions options)
    {
        Debug.Assert(handle is { IsClosed: false, IsInvalid: false });
        Debug.Assert(handle.IsAsync <= ((options.Flags & FileOptions.Asynchronous) != 0));

        _handle = handle;
        _options = options;
    }

#if DEBUG
    ~FileSession()
    {
        Debug.Fail($"{nameof(FileSession)} should have been disposed.");
    }
#endif

    [Owned]
    public SafeFileHandle Handle
    {
        get
        {
            RequireNotDisposed();
            return _handle;
        }
    }

    public FileHandleOptions Options
    {
        get
        {
            RequireNotDisposed();
            return _options;
        }
    }

    public FileHandleType Type
    {
        get
        {
            RequireNotDisposed();
            return _handle.Type;
        }
    }

    public bool IsAsync
    {
        get
        {
            RequireNotDisposed();

#if NET6_0_OR_GREATER
            return _handle.IsAsync;
#else
            return (_options.Flags & FileOptions.Asynchronous) != 0;
#endif
        }
    }

    public bool IsOpen =>
        _handle is not null && _handle.IsOpen;

    public bool IsClosed =>
        _handle is null || _handle.IsClosed;

    public bool CanRead
    {
        get
        {
            RequireNotDisposed();
            return ((RawFileSystemAccess)_options.Access & RawFileSystemAccess.Read) != 0;
        }
    }

    public bool CanWrite
    {
        get
        {
            RequireNotDisposed();
            return ((RawFileSystemAccess)_options.Access & RawFileSystemAccess.Write) != 0;
        }
    }

    public bool CanSeek
    {
        get
        {
            RequireNotDisposed();
            return _handle.CanSeek;
        }
    }

    public string? OpenedPath
    {
        get
        {
            RequireNotDisposed();
            return _handle.OpenedPath;
        }
    }

    [return: OwnershipTransfer]
    public static FileSession Open(string path, FileHandleRequest request) =>
        new(FileOperations.OpenHandle(path, request), request.HandleOptions);

    public static bool TryOpen(
        string path,
        FileHandleRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out FileSession? file,
        bool ignoreMissingDirectory = false)
    {
        file = FileOperations.TryOpenHandle(path, request, out var fileHandle, ignoreMissingDirectory)
            ? new(fileHandle, request.HandleOptions)
            : null;
        return file is not null;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [return: OwnershipTransfer]
    public static FileSession Open(
        PersistentFileId fileId,
        FileHandleRequest request)
    {
        return new(FileOperations.OpenHandle(fileId, request), request.HandleOptions);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryOpen(
        PersistentFileId fileId,
        FileHandleRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out FileSession? file,
        bool ignoreMissingDirectory = false)
    {
        file = FileOperations.TryOpenHandle(fileId, request, out var fileHandle, ignoreMissingDirectory)
            ? new(fileHandle, request.HandleOptions)
            : null;
        return file is not null;
    }

    [return: OwnershipTransfer]
    public static FileSession OpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileHandleRequest request)
    {
        return new(FileOperations.OpenHandleAt(rootDirectory, path, request), request.HandleOptions);
    }

    public static bool TryOpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileHandleRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out FileSession? file,
        bool ignoreMissingDirectory = false)
    {
        file = FileOperations.TryOpenHandleAt(rootDirectory, path, request, out var fileHandle, ignoreMissingDirectory)
            ? new(fileHandle, request.HandleOptions)
            : null;
        return file is not null;
    }

    [return: OwnershipTransfer]
    public static FileSession Reopen([Borrow] FileSession file, FileHandleRequest? request = null)
    {
        var openRequest = request ?? FileHandleRequest.Open(file.Options);
        return new(FileOperations.OpenHandleAt(file.Handle, null, openRequest), openRequest.HandleOptions);
    }

    [return: OwnershipTransfer]
    public static FileSession Duplicate([Borrow] FileSession file) =>
        new(FileOperations.DuplicateHandle(file.Handle), file.Options);

    [return: OwnershipTransfer]
    public static FileSession CreateTempFile(FileSystemAccess access) =>
        CreateTempFile(access, FileHandleOptions.GetDefaultFileShare(access));

    [return: OwnershipTransfer]
    public static FileSession CreateTempFile(
        FileSystemAccess access,
        FileShare share,
        FileHandleType type = FileHandleType.RegularFile,
        FileOptions flags = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var path = FileOperations.GetTempFilePath();
        var request = FileHandleRequest.CreateNew(access, share, type, flags) with { Attributes = attributes };
        return Open(path, request);
    }

    [return: OwnershipTransfer]
    public static FileSession CreateTempFile(
        FileHandleOptions handleOptions,
        FileCreationOptions creationOptions = default)
    {
        handleOptions = handleOptions with { Flags = handleOptions.Flags | FileOptions.DeleteOnClose };
        creationOptions = creationOptions with { Attributes = creationOptions.Attributes | FileAttributes.Temporary };

        var path = FileOperations.GetTempFilePath();
        var request = new FileHandleRequest(FileMode.CreateNew, handleOptions, creationOptions);
        return Open(path, request);
    }

    public string GetPath()
    {
        RequireNotDisposed();
        return FileOperations.GetPath(_handle);
    }

    public FileId GetId()
    {
        RequireNotDisposed();
        return FileOperations.GetId(_handle);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId()
    {
        RequireNotDisposed();
        return FileOperations.GetPersistentId(_handle);
    }

    public FileAttributes GetAttributes()
    {
        RequireNotDisposed();
        return FileOperations.GetAttributes(_handle);
    }

    public void SetAttributes(FileAttributes attributes)
    {
        RequireNotDisposed();
        FileOperations.SetAttributes(_handle, attributes);
    }

    public FileBasicInfo GetBasicInfo()
    {
        RequireNotDisposed();
        return FileOperations.GetBasicInfo(_handle);
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        RequireNotDisposed();
        FileOperations.Move(_handle, destFileName, overwrite);
    }

    public void Delete()
    {
        RequireNotDisposed();
        FileOperations.Delete(_handle);
    }

    [return: OwnershipTransferWhen(nameof(ownsHandle))]
    public CheckedFileStream CreateFileStream(FileAccess access, bool ownsHandle = false, int bufferSize = FileStreamExtensions.DefaultBufferSize)
    {
        RequireNotDisposed();
        Require.ValueHasFlag(Options.Access.ToFileAccess(), access);

        // If the disposal will be left to the file stream, we don't need to assert disposal.
        if (ownsHandle)
            GC.SuppressFinalize(this);

        return _handle.CreateFileStream(access, ownsHandle, bufferSize);
    }

    [return: OwnershipTransfer]
    public SafeFileHandle DetachHandle()
    {
        RequireNotDisposed();

        GC.SuppressFinalize(this);
        var handle = _handle;
        _handle = null!;
        return handle;
    }

    public void Dispose()
    {
        if (_handle is not null)
        {
            GC.SuppressFinalize(this);
            _handle.Dispose();
            _handle = null!;
        }
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }

    public long GetLength()
    {
        RequireNotDisposed();
        return FileOperations.GetLength(_handle);
    }

    public void SetLength(long length)
    {
        RequireNotDisposed();
        FileOperations.SetLength(_handle, length);
    }

    private void RequireNotDisposed()
    {
        Require.NotDisposed(_handle is null, this);
        Require.NotDisposed(_handle.IsClosed, _handle);
    }
}

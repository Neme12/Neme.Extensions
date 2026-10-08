using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.Ownership;
using Neme.Utilities.Contracts;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public sealed class FileReference : IFileObject, IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly string? _openedPath;
    private readonly FileReferenceOptions _options;

    private FileReference(
        [OwnershipTransfer] SafeFileHandle handle,
        string? openedPath,
        FileReferenceOptions options)
    {
        Debug.Assert(!handle.IsClosed && !handle.IsInvalid);
        Debug.Assert(!handle.IsAsync);

        _handle = handle;
        _openedPath = openedPath;
        _options = options;
    }

#if DEBUG
    ~FileReference()
    {
        Debug.Fail($"{nameof(FileReference)} should have been disposed.");
    }
#endif

    [Owned]
    internal SafeFileHandle Handle =>
        _handle;

    public string? OpenedPath
    {
        get
        {
            RequireNotDisposed();
            return _openedPath;
        }
    }

    public FileReferenceOptions Options
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

    public bool IsClosed
    {
        get
        {
            RequireNotDisposed();
            return _handle.IsClosed;
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

    public FileAttributes GetAttributes()
    {
        RequireNotDisposed();

        return FileOperations.GetAttributes(_handle);
    }

    public void SetAttributes(FileAttributes attributes)
    {
        RequireNotDisposed();

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.WriteAttributes, FileShare.All)))
            FileOperations.SetAttributes(handle, attributes);
    }

    public FileBasicInfo GetBasicInfo()
    {
        RequireNotDisposed();

        return FileOperations.GetBasicInfo(_handle);
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        RequireNotDisposed();

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Move(handle, destFileName, overwrite);
    }

    public void Delete()
    {
        RequireNotDisposed();

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.Delete, FileShare.All)))
            FileOperations.Delete(handle);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId()
    {
        RequireNotDisposed();

        return FileOperations.GetPersistentId(_handle);
    }

    public long GetLength()
    {
        RequireNotDisposed();

        return FileOperations.GetLength(_handle);
    }

    public void SetLength(long length)
    {
        RequireNotDisposed();
        Require.ArgumentNotNegative(length);

        using (var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(FileSystemAccess.Write, FileShare.All)))
            FileOperations.SetLength(handle, length);
    }

    [return: OwnershipTransfer]
    public static FileReference CreateFromHandle([Borrow] SafeFileHandle handle, FileReferenceOptions options = default)
    {
        Require.ArgumentNotNull(handle);
        Require.ArgumentValid(handle, !handle.IsClosed && !handle.IsInvalid);

        var handleRequest = GetFileHandleRequest(FileReferenceMode.Open, options);
        var newHandle = FileOperations.ReopenHandle(handle, handleRequest);
        return new FileReference(newHandle, handle.OpenedPath, options);
    }

    [return: OwnershipTransfer]
    public static FileReference Create(string path, FileReferenceRequest request = default)
    {
        Require.ArgumentNotNull(path);

        var handleRequest = GetFileHandleCreationRequest(request);
        var handle = FileOperations.OpenHandle(path, handleRequest);
        return new FileReference(handle, path, request.ReferenceOptions);
    }

    [return: OwnershipTransfer]
    public static FileReference Duplicate([Borrow] FileReference file)
    {
        Require.ArgumentNotNull(file);

        return new(FileOperations.DuplicateHandle(file.Handle), file.OpenedPath, file.Options);
    }

    [return: OwnershipTransfer]
    public static FileReference CreateTempFile() =>
        CreateTempFile();

    [return: OwnershipTransfer]
    public static FileReference CreateTempFile(
        FileHandleType type = FileHandleType.RegularFile,
        FileReferenceFlags flags = FileReferenceFlags.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var path = FileOperations.GetTempFilePath();
        var referenceRequest = FileReferenceRequest.CreateNew(new(type, flags), new(attributes));
        return Create(path, referenceRequest);
    }

    [return: OwnershipTransfer]
    public static FileReference CreateTempFile(
        FileReferenceOptions referenceOptions,
        FileCreationOptions creationOptions = default)
    {
        referenceOptions = referenceOptions with { Flags = referenceOptions.Flags | FileReferenceFlags.DeleteOnClose };
        creationOptions = creationOptions with { Attributes = creationOptions.Attributes | FileAttributes.Temporary };

        var path = FileOperations.GetTempFilePath();
        var request = FileReferenceRequest.CreateNew(referenceOptions, creationOptions);
        return Create(path, request);
    }

    [return: OwnershipTransfer]
    public FileSession OpenSession(FileHandleOptions options)
    {
        RequireNotDisposed();

        var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(options));
        return new FileSession(handle, options);
    }

    [return: OwnershipTransfer]
    public SafeFileHandle OpenHandle(FileHandleOptions options)
    {
        RequireNotDisposed();

        var handle = FileOperations.ReopenHandle(_handle, FileHandleRequest.Open(options));
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

    private static FileHandleRequest GetFileHandleRequest(
        FileReferenceMode mode,
        FileReferenceOptions referenceOptions)
    {
        return new FileHandleRequest(
            mode.ToFileMode(),
            FileSystemAccess.ReadAttributes,
            FileShare.All,
            referenceOptions.Type,
            referenceOptions.Flags.HasFlag(FileReferenceFlags.DeleteOnClose)
                ? FileOptions.DeleteOnClose
                : FileOptions.None);
    }

    private static FileHandleRequest GetFileHandleCreationRequest(FileReferenceRequest request)
    {
        return GetFileHandleRequest(request.Mode, request.ReferenceOptions) with
        {
            CreationOptions = request.CreationOptions,
        };
    }

    private void RequireNotDisposed()
    {
        Require.NotDisposed(_handle is null, this);
    }
}

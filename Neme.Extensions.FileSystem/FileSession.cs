using Microsoft.Win32.SafeHandles;
using Neme.Extensions.IO;
using Neme.Extensions.Ownership;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public sealed class FileSession : IDisposable
{
    [Owned]
    private SafeFileHandle _handle;
    private readonly FileHandleOptions _options;

    internal FileSession([OwnershipTransfer] SafeFileHandle handle, FileHandleOptions options)
    {
        Debug.Assert(handle is { IsClosed: false, IsInvalid: false });
        Debug.Assert(handle.IsAsync == ((options.Flags & FileOptions.Asynchronous) != 0));

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
            ObjectDisposedException.ThrowIf(_handle is null, this);
            return _handle;
        }
    }

    public FileHandleOptions Options
    {
        get
        {
            ObjectDisposedException.ThrowIf(_handle is null, this);
            return _options;
        }
    }

    public bool IsAsync
    {
        get
        {
#if NET6_0_OR_GREATER
            return _handle.IsAsync;
#else
            return (_options.Flags & FileOptions.Asynchronous) != 0;
#endif
        }
    }

    public bool IsClosed =>
        _handle.IsClosed;

    public bool CanRead =>
        ((RawFileSystemAccess)_options.Access & RawFileSystemAccess.Read) != 0;

    public bool CanWrite =>
        ((RawFileSystemAccess)_options.Access & RawFileSystemAccess.Write) != 0;

    [return: OwnershipTransfer]
    public static FileSession Open(string path, FileOpenRequest request) =>
        new(FileIO.OpenHandle(path, request), request.HandleOptions);

    public static bool TryOpen(
        string path,
        FileOpenRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out FileSession? file,
        bool ignoreMissingDirectory = false)
    {
        file = FileIO.TryOpenHandle(path, request, out var fileHandle, ignoreMissingDirectory)
            ? new(fileHandle, request.HandleOptions)
            : null;
        return file is not null;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [return: OwnershipTransfer]
    public static FileSession Open(
        PersistentFileId fileId,
        FileOpenRequest request)
    {
        return new(FileIO.OpenHandle(fileId, request), request.HandleOptions);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryOpen(
        PersistentFileId fileId,
        FileOpenRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out FileSession? file,
        bool ignoreMissingDirectory = false)
    {
        file = FileIO.TryOpenHandle(fileId, request, out var fileHandle, ignoreMissingDirectory)
            ? new(fileHandle, request.HandleOptions)
            : null;
        return file is not null;
    }

    [return: OwnershipTransfer]
    public static FileSession OpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileOpenRequest request)
    {
        return new(FileIO.OpenHandleAt(rootDirectory, path, request), request.HandleOptions);
    }

    public static bool TryOpenAt(
        [Borrow] SafeFileHandle? rootDirectory,
        string? path,
        FileOpenRequest request,
        [NotNullWhen(true)][OwnershipTransfer] out FileSession? file,
        bool ignoreMissingDirectory = false)
    {
        file = FileIO.TryOpenHandleAt(rootDirectory, path, request, out var fileHandle, ignoreMissingDirectory)
            ? new(fileHandle, request.HandleOptions)
            : null;
        return file is not null;
    }

    [return: OwnershipTransfer]
    public static FileSession Reopen([Borrow] FileSession file, FileOpenRequest? request = null)
    {
        var openRequest = request ?? FileOpenRequest.Open(file.Options);
        return new(FileIO.OpenHandleAt(file.Handle, null, openRequest), openRequest.HandleOptions);
    }

    [return: OwnershipTransfer]
    public static FileSession Duplicate([Borrow] FileSession file) =>
        new(FileIO.DuplicateHandle(file.Handle), file.Options);

    [return: OwnershipTransfer]
    public static FileSession CreateTempFile(FileSystemAccess access) =>
        CreateTempFile(access, FileOpenRequest.GetDefaultFileShare(access));

    [return: OwnershipTransfer]
    public static FileSession CreateTempFile(
        FileSystemAccess access,
        FileShare share,
        FileOptions options = FileOptions.DeleteOnClose,
        FileAttributes attributes = FileAttributes.Temporary)
    {
        var (path, request) = FileIO.GetTempFilePathAndRequest(access, share, options, attributes);
        return Open(path, request);
    }

    public string GetPath()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetPath(_handle);
    }

    public FileId GetId()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetId(_handle);
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetPersistentId(_handle);
    }

    public FileAttributes GetAttributes()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetAttributes(_handle);
    }

    public void SetAttributes(FileAttributes attributes)
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        FileIO.SetAttributes(_handle, attributes);
    }

    public FileBasicInfo GetBasicInfo()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.GetBasicInfo(_handle);
    }

    public void Move(string destFileName, bool overwrite = false)
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        FileIO.Move(_handle, destFileName, overwrite);
    }

    public void Delete()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);
        FileIO.Delete(_handle);
    }

    [return: OwnershipTransferWhen(nameof(ownsHandle))]
    public CheckedFileStream CreateFileStream(bool ownsHandle = false, int bufferSize = FileStreamExtensions.DefaultBufferSize)
    {
        // If the disposal will be left to the file stream, we don't need to assert disposal.
        if (ownsHandle)
            GC.SuppressFinalize(this);

        ObjectDisposedException.ThrowIf(_handle is null, this);
        return FileIO.CreateFileStream(_handle, _options.Access.ToFileAccess(), ownsHandle, bufferSize);
    }

    [return: OwnershipTransfer]
    public SafeFileHandle DetachHandle()
    {
        ObjectDisposedException.ThrowIf(_handle is null, this);

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
}

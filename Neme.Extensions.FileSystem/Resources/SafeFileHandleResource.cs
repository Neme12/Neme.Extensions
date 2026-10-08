using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.SafeHandles;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Resources;

internal sealed class SafeFileHandleResource : IFileResource
{
    private readonly SafeFileHandle _handle;

    public SafeFileHandleResource(SafeFileHandle handle)
    {
        Debug.AssertNotNull(handle);
        Debug.Assert(handle.IsOpen);

        _handle = handle;
    }

    public SafeFileHandle Handle =>
        _handle;

    public string? OpenedPath =>
        _handle.OpenedPath;

    public bool IsOpen =>
        _handle.IsOpen;

    public bool IsClosed =>
        _handle.IsClosed;

    public void Delete() =>
        FileOperations.Delete(_handle);

    public FileAttributes GetAttributes() =>
        FileOperations.GetAttributes(_handle);

    public FileBasicInfo GetBasicInfo() =>
        FileOperations.GetBasicInfo(_handle);

    public FileId GetId() =>
        FileOperations.GetId(_handle);

    public long GetLength() =>
        FileOperations.GetLength(_handle);

    public string GetPath() =>
        FileOperations.GetPath(_handle);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId() =>
        FileOperations.GetPersistentId(_handle);

    public void Move(string destFileName, bool overwrite = false) =>
        FileOperations.Move(_handle, destFileName, overwrite);

    public void SetAttributes(FileAttributes attributes) =>
        FileOperations.SetAttributes(_handle, attributes);

    public void SetLength(long length) =>
        FileOperations.SetLength(_handle, length);

    public void Dispose() =>
        _handle.Dispose();

    public ValueTask DisposeAsync()
    {
        _handle.Dispose();
        return default;
    }
}

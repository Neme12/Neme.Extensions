using Microsoft.Win32.SafeHandles;
using Neme.Extensions.Contracts;
using Neme.Extensions.FileSystem.Resources;
using Neme.Extensions.FileSystem.SafeHandles;
using Neme.Extensions.SafeHandles;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class FileStreamResource : IFileResource
{
    private readonly FileStream _fileStream;

    private SafeFileHandle Handle =>
        _fileStream.SafeFileHandle;

    public FileStreamResource(FileStream fileStream)
    {
        Debug.AssertNotNull(fileStream);
        Debug.Assert(fileStream.SafeFileHandle.IsOpen);

        _fileStream = fileStream;
    }

    public FileStream FileStream =>
        _fileStream;

    public string? OpenedPath =>
        Handle.OpenedPath;

    public bool IsOpen =>
        Handle.IsOpen;

    public bool IsClosed =>
        Handle.IsClosed;

    public void Delete() =>
        FileOperations.Delete(Handle);

    public FileAttributes GetAttributes() =>
        FileOperations.GetAttributes(Handle);

    public FileBasicInfo GetBasicInfo() =>
        FileOperations.GetBasicInfo(Handle);

    public FileId GetId() =>
        FileOperations.GetId(Handle);

    public long GetLength() =>
        FileOperations.GetLength(Handle);

    public string GetPath() =>
        FileOperations.GetPath(Handle);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId() =>
        FileOperations.GetPersistentId(Handle);

    public void Move(string destFileName, bool overwrite = false) =>
        FileOperations.Move(Handle, destFileName, overwrite);

    public void SetAttributes(FileAttributes attributes) =>
        FileOperations.SetAttributes(Handle, attributes);

    public void SetLength(long length) =>
        FileOperations.SetLength(Handle, length);

    public void Dispose() =>
        _fileStream.Dispose();

    public ValueTask DisposeAsync() =>
        _fileStream.DisposeAsync();
}

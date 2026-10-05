using Microsoft.Win32.SafeHandles;
using Neme.Extensions.FileSystem.SafeHandles;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem.Internal;

internal sealed class FileStreamAdapter : IFileObject
{
    private readonly FileStream _fileStream;
    private readonly SafeFileHandle _hahdle;

    public FileStreamAdapter(FileStream fileStream)
    {
        _fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
        _hahdle = fileStream.SafeFileHandle;
    }

    public FileStream FileStream =>
        _fileStream;

    public string? OpenedPath =>
        _hahdle.OpenedPath;

    public bool IsClosed =>
        _hahdle.IsClosed;

    public void Delete() =>
        FileOperations.Delete(_hahdle);

    public void Dispose() =>
        _fileStream.Dispose();

    public ValueTask DisposeAsync() =>
        _fileStream.DisposeAsync();

    public FileAttributes GetAttributes() =>
        FileOperations.GetAttributes(_hahdle);

    public FileBasicInfo GetBasicInfo() =>
        FileOperations.GetBasicInfo(_hahdle);

    public FileId GetId() =>
        FileOperations.GetId(_hahdle);

    public long GetLength() =>
        FileOperations.GetLength(_hahdle);

    public string GetPath() =>
        FileOperations.GetPath(_hahdle);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId() =>
        FileOperations.GetPersistentId(_hahdle);

    public void Move(string destFileName, bool overwrite = false) =>
        FileOperations.Move(_hahdle, destFileName, overwrite);

    public void SetAttributes(FileAttributes attributes) =>
        FileOperations.SetAttributes(_hahdle, attributes);

    public void SetLength(long length) =>
        FileOperations.SetLength(_hahdle, length);
}

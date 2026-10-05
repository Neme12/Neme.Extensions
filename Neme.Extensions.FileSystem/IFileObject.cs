using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

public interface IFileObject : IDisposable
{
    public string? OpenedPath { get; }

    public bool IsClosed { get; }

    public string GetPath();

    public FileId GetId();

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public PersistentFileId GetPersistentId();

    public FileAttributes GetAttributes();

    public void SetAttributes(FileAttributes attributes);

    public FileBasicInfo GetBasicInfo();

    public long GetLength();

    public void SetLength(long length);

    public void Move(string destFileName, bool overwrite = false);

    public void Delete();
}

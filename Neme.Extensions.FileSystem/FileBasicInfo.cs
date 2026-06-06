using NodaTime;

namespace Neme.Extensions.FileSystem;

public readonly record struct FileBasicInfo
{
    public FileAttributes Attributes { get; init; }

    public Instant? CreationTime { get; init; }

    public Instant LastAccessTime { get; init; }

    public Instant LastWriteTime { get; init; }

    public long Size { get; init; }
}

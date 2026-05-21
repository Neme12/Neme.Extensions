namespace Neme.Extensions.FileSystem;

public readonly record struct FsFileId
{
    internal ulong VolumeSerialNumber { get; init; }
    internal ulong FileIdHigh { get; init; }
    internal ulong FileIdLow { get; init; }
}

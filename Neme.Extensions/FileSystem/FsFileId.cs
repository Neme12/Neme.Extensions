namespace Neme.Extensions.FileSystem;

public readonly record struct FsFileId
{
    private readonly ulong _volumeSerialNumber;
    private readonly ulong _fileIdHigh;
    private readonly ulong _fileIdLow;

    internal FsFileId(ulong volumeSerialNumber, ulong fileIdHigh, ulong fileIdLow)
    {
        _volumeSerialNumber = volumeSerialNumber;
        _fileIdHigh = fileIdHigh;
        _fileIdLow = fileIdLow;
    }

    internal ulong VolumeSerialNumber => _volumeSerialNumber;
    internal ulong FileIdHigh => _fileIdHigh;
    internal ulong FileIdLow => _fileIdLow;
}

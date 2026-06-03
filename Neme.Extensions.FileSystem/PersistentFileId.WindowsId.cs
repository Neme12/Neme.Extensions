namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
    internal sealed record class WindowsId : PlatformId
    {
        internal readonly ulong VolumeSerialNumber;
        internal readonly ulong FileIdHigh;
        internal readonly ulong FileIdLow;

        public WindowsId(ulong volumeSerialNumber, ulong fileIdHigh, ulong fileIdLow)
        {
            VolumeSerialNumber = volumeSerialNumber;
            FileIdHigh = fileIdHigh;
            FileIdLow = fileIdLow;
        }

        public override string ToString() =>
            $"v1:w:{VolumeSerialNumber:x16}:{FileIdHigh:x16}:{FileIdLow:x16}";
    }
}

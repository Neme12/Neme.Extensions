using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct WindowsId
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

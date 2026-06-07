using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct FileId
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
    }
}

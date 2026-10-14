using System.Globalization;

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

        public static WindowsId Parse(string[] parts)
        {
            if (parts.Length != 5 ||
                parts[0] != "v1" ||
                parts[1] != "w" ||
                parts[2].Length != 16 ||
                parts[3].Length != 16 ||
                parts[4].Length != 16)
            {
                throw new FormatException("Invalid WindowsId format.");
            }

            var volumeSerialNumber = ulong.Parse(parts[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var fileIdHigh = ulong.Parse(parts[3], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var fileIdLow = ulong.Parse(parts[4], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            return new WindowsId(volumeSerialNumber, fileIdHigh, fileIdLow);
        }

        public override string ToString() =>
            $"v1:w:{VolumeSerialNumber:x16}:{FileIdHigh:x16}:{FileIdLow:x16}";
    }
}

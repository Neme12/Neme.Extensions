using System.Text;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
    internal sealed record class LinuxId : PlatformId
    {
        internal readonly int MountId;
        internal readonly int FileType;
        internal readonly InlineByteArray Buffer;
        internal readonly byte BufferLength;

        public LinuxId(
            int mountId,
            int fileType,
            InlineByteArray buffer,
            byte bufferLength)
        {
            MountId = mountId;
            FileType = fileType;
            Buffer = buffer;
            BufferLength = bufferLength;
        }

        public override string ToString()
        {
            var builder = new StringBuilder($"v1:l:{(uint)MountId:x8}:{(uint)FileType:x8}:");
            builder.EnsureCapacity(builder.Length + (BufferLength * 2));

            Buffer.WithSpan(static (span, state) =>
            {
                for (int i = 0; i < state.Length; i++)
                    state.Builder.Append($"{span[i]:x2}");
            }, (Builder: builder, Length: (int)BufferLength));

            return builder.ToString();
        }
    }
}

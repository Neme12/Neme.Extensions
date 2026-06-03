using Neme.Extensions.Buffers;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
    internal sealed record class LinuxId : PlatformId
    {
        internal readonly string MountPath;
        internal readonly int FileType;
        internal readonly InlineByteArray Buffer;
        internal readonly byte BufferLength;

        public LinuxId(
            string mountPath,
            int fileType,
            InlineByteArray buffer,
            byte bufferLength)
        {
            MountPath = mountPath;
            FileType = fileType;
            Buffer = buffer;
            BufferLength = bufferLength;
        }

        public override string ToString()
        {
            return Buffer.WithSpan(static (span, fileId) =>
            {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                var mountPathBufferLength = Encoding.UTF8.GetMaxByteCount(fileId.MountPath.Length);
                using var mountPathBuffer = ArrayPool<byte>.Shared.RentLeaseOrStackalloc(
                    mountPathBufferLength,
                    mountPathBufferLength < Stackalloc.MaxLength<byte>() ? stackalloc byte[mountPathBufferLength] : default);

                var bytesWritten = Encoding.UTF8.GetBytes(fileId.MountPath.AsSpan(), mountPathBuffer.Buffer);
                var mountPathBytes = mountPathBuffer.Buffer[0..bytesWritten];
#else
                var mountPathBytes = Encoding.UTF8.GetBytes(fileId.MountPath);
#endif

                var builder = new DefaultInterpolatedStringHandler(literalLength: 5 + mountPathBytes.Length * 2 + 1 + 8 + 1 + fileId.BufferLength * 2, formattedCount: 0);
                builder.AppendLiteral("v1:l:");

                foreach (var @byte in mountPathBytes)
                    builder.AppendFormatted(@byte, "x2");

                builder.AppendLiteral(":");

                builder.AppendFormatted((uint)fileId.FileType, "x8");

                builder.AppendLiteral(":");

                for (int i = 0; i < fileId.BufferLength; i++)
                    builder.AppendFormatted(span[i], "x2");

                return builder.ToStringAndClear();
            }, this);
        }
    }
}

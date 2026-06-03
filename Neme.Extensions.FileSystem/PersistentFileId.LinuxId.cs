using Neme.Extensions.Buffers;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
    internal sealed record class LinuxId : PlatformId
    {
        internal readonly string MountPath;
        internal readonly int FileType;
        internal readonly byte[]? Buffer;
        internal readonly InlineByteArray InlineBuffer;
        internal readonly byte InlineBufferLength;

        public unsafe LinuxId(
            string mountPath,
            int fileType,
            Span<byte> buffer)
        {
            MountPath = mountPath;
            FileType = fileType;

            if (buffer.Length <= InlineByteArray.Length)
            {
#if NET8_0_OR_GREATER
                buffer.CopyTo(MemoryMarshal.CreateSpan(ref InlineBuffer.byte0, InlineByteArray.Length));
#else
                fixed (byte* inlineBufferPtr = InlineBuffer.bytes)
                    buffer.CopyTo(new Span<byte>(inlineBufferPtr, InlineByteArray.Length));
#endif
                InlineBufferLength = (byte)buffer.Length;
            }
            else
            {
                Buffer = buffer.ToArray();
                InlineBufferLength = 0;
            }
        }

        public bool Equals(LinuxId? other)
        {
            if (other is null)
                return false;

            if (Buffer is not null)
            {
                return
                    MountPath == other.MountPath &&
                    FileType == other.FileType &&
                    other.Buffer is not null &&
                    Buffer.AsSpan().SequenceEqual(other.Buffer);
            }
            else
            {
                return
                    MountPath == other.MountPath &&
                    FileType == other.FileType &&
                    other.Buffer is null &&
                    InlineBuffer.Equals(other.InlineBuffer) &&
                    InlineBufferLength == other.InlineBufferLength;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(MountPath);
            hashCode.Add(FileType);

            if (Buffer is not null)
            {
#if NET6_0_OR_GREATER
                hashCode.AddBytes(Buffer);
#else
                foreach (var @byte in Buffer)
                    hashCode.Add(@byte);
#endif
            }
            else
            {
                hashCode.Add(InlineBuffer);
                hashCode.Add(InlineBufferLength);
            }

            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return InlineBuffer.WithSpan(static (span, fileId) =>
            {
                var bufferLength = fileId.Buffer?.Length ?? fileId.InlineBufferLength;

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

                var builder = new DefaultInterpolatedStringHandler(literalLength: 5 + mountPathBytes.Length * 2 + 1 + 8 + 1 + bufferLength * 2, formattedCount: 0);
                builder.AppendLiteral("v1:l:");

                foreach (var @byte in mountPathBytes)
                    builder.AppendFormatted(@byte, "x2");

                builder.AppendLiteral(":");

                builder.AppendFormatted((uint)fileId.FileType, "x8");

                builder.AppendLiteral(":");

                for (int i = 0; i < bufferLength; i++)
                {
                    if (fileId.Buffer is not null)
                        builder.AppendFormatted(fileId.Buffer[i], "x2");
                    else
                        builder.AppendFormatted(span[i], "x2");
                }

                return builder.ToStringAndClear();
            }, this);
        }
    }
}

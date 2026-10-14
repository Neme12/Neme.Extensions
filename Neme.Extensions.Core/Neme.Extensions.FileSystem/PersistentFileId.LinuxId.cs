using Neme.Extensions.Buffers;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
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

        public static LinuxId Parse(string[] parts)
        {
            if (parts.Length != 5 ||
                parts[0] != "v1" ||
                parts[1] != "l" ||
                (parts[2].Length % 2) != 0 ||
                parts[3].Length != 8 ||
                (parts[4].Length % 2) != 0)
            {
                throw new FormatException("Invalid LinuxId format.");
            }

            var mountPathBufferLength = parts[2].Length / 2;
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            using var mountPathBuffer = ArrayPool<byte>.Shared.RentLeaseOrStackalloc(
                mountPathBufferLength,
                mountPathBufferLength < Stackalloc.MaxLength<byte>() ? stackalloc byte[mountPathBufferLength] : default);

            var mounthPathBytes = mountPathBuffer.Buffer[0..mountPathBufferLength];
            FromHexString(parts[2], mounthPathBytes);
            var mountPath = Encoding.UTF8.GetString(mounthPathBytes);
#else
            var mountPathBytes = new byte[mountPathBufferLength];
            FromHexString(parts[2], mountPathBytes);
            var mountPath = Encoding.UTF8.GetString(mountPathBytes);
#endif

            var fileType = int.Parse(parts[3], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            var bufferLength = parts[4].Length / 2;
            using var bufferBuffer = ArrayPool<byte>.Shared.RentLeaseOrStackalloc(
                bufferLength,
                bufferLength < Stackalloc.MaxLength<byte>() ? stackalloc byte[bufferLength] : default);

            var buffer = bufferBuffer.Buffer[0..bufferLength];

            FromHexString(parts[4], buffer);

            return new LinuxId(mountPath, fileType, buffer);
        }

        private static void FromHexString(string hex, Span<byte> destination)
        {
            Debug.Assert(hex.Length == destination.Length * 2);
 
#if NET9_0_OR_GREATER
            var status = Convert.FromHexString(hex, destination, out var charsConsumed, out var bytesWritten);
            Debug.Assert(status == OperationStatus.Done);
            Debug.Assert(charsConsumed == hex.Length);
            Debug.Assert(bytesWritten == destination.Length);
#elif NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            for (int i = 0; i < destination.Length; i++)
                destination[i] = byte.Parse(hex.AsSpan(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
#else
            for (int i = 0; i < destination.Length; i++)
                destination[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
#endif
        }

        public override string ToString()
        {
            var bufferLength = Buffer?.Length ?? InlineBufferLength;

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            var mountPathBufferLength = Encoding.UTF8.GetMaxByteCount(MountPath.Length);
            using var mountPathBuffer = ArrayPool<byte>.Shared.RentLeaseOrStackalloc(
                mountPathBufferLength,
                mountPathBufferLength < Stackalloc.MaxLength<byte>() ? stackalloc byte[mountPathBufferLength] : default);

            var bytesWritten = Encoding.UTF8.GetBytes(MountPath.AsSpan(), mountPathBuffer.Buffer);
            var mountPathBytes = mountPathBuffer.Buffer[0..bytesWritten];
#else
            var mountPathBytes = Encoding.UTF8.GetBytes(MountPath);
#endif

            var builder = new DefaultInterpolatedStringHandler(literalLength: 5 + mountPathBytes.Length * 2 + 1 + 8 + 1 + bufferLength * 2, formattedCount: 0);
            builder.AppendLiteral("v1:l:");

            foreach (var @byte in mountPathBytes)
                builder.AppendFormatted(@byte, "x2");

            builder.AppendLiteral(":");

            builder.AppendFormatted((uint)FileType, "x8");

            builder.AppendLiteral(":");

            for (int i = 0; i < bufferLength; i++)
            {
                if (Buffer is not null)
                {
                    builder.AppendFormatted(Buffer[i], "x2");
                }
                else
                {
                    var bufferSpan = InlineBuffer.AsSpan();
                    builder.AppendFormatted(bufferSpan[i], "x2");
                }
            }

            return builder.ToStringAndClear();
        }
    }
}

using Neme.Extensions.Buffers;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Neme.Extensions.FileSystem;

[StructLayout(LayoutKind.Explicit)]
public readonly record struct PersistentFileId : IEquatable<PersistentFileId>
{
    [FieldOffset(0)]
    private readonly FileIdKind _fileIdKind;

    [FieldOffset(8)]
    private readonly WindowsId _windowsFileId;
    [FieldOffset(8)]
    private readonly LinuxId _linuxFileId;

    private enum FileIdKind : byte
    {
        Windows = 0,
        Linux = 1,
    }

    private PersistentFileId(WindowsId windowsFileId)
    {
        _fileIdKind = FileIdKind.Windows;
        _windowsFileId = windowsFileId;
    }

    private PersistentFileId(LinuxId linuxFileId)
    {
        _fileIdKind = FileIdKind.Linux;
        _linuxFileId = linuxFileId;
    }

    internal static PersistentFileId FromWindowsId(WindowsId windowsFileId) =>
        new PersistentFileId(windowsFileId);

    internal static PersistentFileId FromLinuxId(LinuxId linuxFileId) =>
        new PersistentFileId(linuxFileId);

    internal WindowsId WindowsFileId
    {
        get
        {
            if (_fileIdKind != FileIdKind.Windows)
                throw new InvalidOperationException();

            return _windowsFileId;
        }
    }

    internal LinuxId LinuxFileId
    {
        get
        {
            if (_fileIdKind != FileIdKind.Linux)
                throw new InvalidOperationException();

            return _linuxFileId;
        }
    }

    public bool Equals(PersistentFileId other)
    {
        if (_fileIdKind != other._fileIdKind)
            return false;

        return _fileIdKind switch
        {
            FileIdKind.Windows => WindowsFileId.Equals(other.WindowsFileId),
            FileIdKind.Linux => LinuxFileId.Equals(other.LinuxFileId),
            _ => throw new UnreachableException(),
        };
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_fileIdKind);

        switch (_fileIdKind)
        {
            case FileIdKind.Windows:
                hashCode.Add(_windowsFileId);
                break;

            case FileIdKind.Linux:
                hashCode.Add(_linuxFileId);
                break;

            default:
                throw new UnreachableException();
        }

        return hashCode.ToHashCode();
    }

    public override string ToString() =>
        _fileIdKind switch
        {
            FileIdKind.Windows => _windowsFileId.ToString(),
            FileIdKind.Linux => _linuxFileId.ToString(),
            _ => throw new UnreachableException(),
        };

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

    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct LinuxId
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

#if NET8_0_OR_GREATER
    [InlineArray(128)]
#endif
    internal unsafe struct InlineByteArray : IEquatable<InlineByteArray>
    {
#if NET8_0_OR_GREATER
        public byte byte0;

        public T WithSpan<T, TState>(SpanFunc<byte, TState, T> action, TState state)
        {
            var span = MemoryMarshal.CreateSpan(ref byte0, 128);
            return action(span, state);
        }

        public void WithSpan<TState>(SpanAction<byte, TState> action, TState state)
        {
            var span = MemoryMarshal.CreateSpan(ref byte0, 128);
            action(span, state);
        }

        public bool Equals(InlineByteArray other)
        {
            var span = MemoryMarshal.CreateSpan(ref byte0, 128);
            var otherSpan = MemoryMarshal.CreateSpan(ref other.byte0, 128);
            return span.SequenceEqual(otherSpan);
        }
#else
        public fixed byte bytes[128];

        public T WithSpan<T, TState>(SpanFunc<byte, TState, T> action, TState state)
        {
            fixed (byte* ptr = bytes)
            {
                var span = new Span<byte>(ptr, 128);
                return action(span, state);
            }
        }

        public void WithSpan<TState>(SpanAction<byte, TState> action, TState state)
        {
            fixed (byte* ptr = bytes)
            {
                var span = new Span<byte>(ptr, 128);
                action(span, state);
            }
        }

        public bool Equals(InlineByteArray other)
        {
            fixed (byte* ptr = bytes)
            {
                var span = new Span<byte>(ptr, 128);
                var otherSpan = new Span<byte>(other.bytes, 128);
                return span.SequenceEqual(otherSpan);
            }
        }
#endif

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is InlineByteArray other && Equals(other);

        public override int GetHashCode()
        {
            return WithSpan(static (span, _) =>
            {
                var values = MemoryMarshal.Cast<byte, ulong>(span);

                var hashCode = new HashCode();

                foreach (var value in values)
                    hashCode.Add(value);

                return hashCode.ToHashCode();
            }, default(ValueTuple));
        }
    }
}

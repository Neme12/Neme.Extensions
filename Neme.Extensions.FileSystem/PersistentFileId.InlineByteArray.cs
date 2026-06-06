using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
#if NET8_0_OR_GREATER
    [InlineArray(Length)]
#endif
    internal unsafe struct InlineByteArray : IEquatable<InlineByteArray>
    {
        public const int Length = 16;

#if NET8_0_OR_GREATER
        public byte byte0;

        public Span<byte> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref byte0, Length);
        }
#elif NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        public fixed byte bytes[Length];

        public Span<byte> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref bytes[0], Length);
        }
#else
        [FixedAddressValueType]
        public fixed byte bytes[Length];

        public Span<byte> AsSpan()
        {
            return new Span<byte>(Unsafe.AsPointer(ref bytes[0]), Length);
        }
#endif

        public bool Equals(InlineByteArray other)
        {
            var span = this.AsSpan();
            var otherSpan = other.AsSpan();
            return span.SequenceEqual(otherSpan);
        }

        public override bool Equals([NotNullWhen(true)] object? obj) =>
            obj is InlineByteArray other && Equals(other);

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            foreach (var value in AsSpan())
                hashCode.Add(value);

            return hashCode.ToHashCode();
        }
    }
}

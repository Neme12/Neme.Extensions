using Neme.Extensions.Buffers;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
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

using Neme.Extensions.Buffers;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neme.Extensions.FileSystem;

public readonly partial record struct PersistentFileId
{
#if NET8_0_OR_GREATER
    [InlineArray(16)]
#endif
    internal unsafe struct InlineByteArray : IEquatable<InlineByteArray>
    {
        public const int Length = 16;

#if NET8_0_OR_GREATER
        public byte byte0;

        public T WithSpan<T, TState>(SpanFunc<byte, TState, T> action, TState state)
        {
            var span = MemoryMarshal.CreateSpan(ref byte0, Length);
            return action(span, state);
        }

        public void WithSpan<TState>(SpanAction<byte, TState> action, TState state)
        {
            var span = MemoryMarshal.CreateSpan(ref byte0, Length);
            action(span, state);
        }

        public bool Equals(InlineByteArray other)
        {
            var span = MemoryMarshal.CreateSpan(ref byte0, Length);
            var otherSpan = MemoryMarshal.CreateSpan(ref other.byte0, Length);
            return span.SequenceEqual(otherSpan);
        }
#else
        public fixed byte bytes[16];

        public T WithSpan<T, TState>(SpanFunc<byte, TState, T> action, TState state)
        {
            fixed (byte* ptr = bytes)
            {
                var span = new Span<byte>(ptr, Length);
                return action(span, state);
            }
        }

        public void WithSpan<TState>(SpanAction<byte, TState> action, TState state)
        {
            fixed (byte* ptr = bytes)
            {
                var span = new Span<byte>(ptr, Length);
                action(span, state);
            }
        }

        public bool Equals(InlineByteArray other)
        {
            fixed (byte* ptr = bytes)
            {
                var span = new Span<byte>(ptr, Length);
                var otherSpan = new Span<byte>(other.bytes, Length);
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

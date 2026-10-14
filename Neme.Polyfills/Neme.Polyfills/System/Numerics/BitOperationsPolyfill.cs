using System.Runtime.CompilerServices;

namespace System.Numerics;

#if NETCOREAPP3_0_OR_GREATER
public static class BitOperationsPolyfill
{
    extension(BitOperations)
    {
#else
public static class BitOperations
{
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(int value) =>
#if NET6_0_OR_GREATER
            BitOperations.IsPow2(value);
#else
            (value & (value - 1)) == 0 && value > 0;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static bool IsPow2(uint value) =>
#if NET6_0_OR_GREATER
            BitOperations.IsPow2(value);
#else
            (value & (value - 1)) == 0 && value != 0;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(long value) =>
#if NET6_0_OR_GREATER
            BitOperations.IsPow2(value);
#else
            (value & (value - 1)) == 0 && value > 0;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static bool IsPow2(ulong value) =>
#if NET6_0_OR_GREATER
            BitOperations.IsPow2(value);
#else
            (value & (value - 1)) == 0 && value != 0;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(nint value) =>
#if NET7_0_OR_GREATER
            BitOperations.IsPow2(value);
#else
            (value & (value - 1)) == 0 && value > 0;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static bool IsPow2(nuint value) =>
#if NET7_0_OR_GREATER
            BitOperations.IsPow2(value);
#else
            (value & (value - 1)) == 0 && value != 0;
#endif


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CLSCompliant(false)]
        public static uint RoundUpToPowerOf2(uint value)
        {
#if NET6_0_OR_GREATER
            return BitOperations.RoundUpToPowerOf2(value);
#else
            // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
#endif
        }
#if NETCOREAPP3_0_OR_GREATER
    }
#endif
}

using System.Runtime.CompilerServices;

namespace System;

public static class EnumPolyfill
{
    extension(Enum)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined<TEnum>(TEnum value) where TEnum : struct, Enum
        {
    #if NET5_0_OR_GREATER
            return Enum.IsDefined(value);
    #else
            return Enum.IsDefined(typeof(TEnum), value);
    #endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? GetName<TEnum>(TEnum value) where TEnum : struct, Enum
        {
#if NET5_0_OR_GREATER
            return Enum.GetName(value);
#else
            return Enum.GetName(typeof(TEnum), value);
#endif
        }
    }
}

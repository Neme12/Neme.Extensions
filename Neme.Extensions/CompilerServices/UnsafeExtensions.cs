using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.CompilerServices;

internal static class UnsafeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TTo OutAs<TFrom, TTo>([UnscopedRef] out TFrom source)
    {
        Unsafe.SkipInit(out source);
        return ref Unsafe.As<TFrom, TTo>(ref source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TTo InAs<TFrom, TTo>(in TFrom source)
    {
        return ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in source));
    }
}

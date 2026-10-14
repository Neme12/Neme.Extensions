using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public static class ImmutableCollectionsMarshalPolyfill
{
    public static ImmutableArray<T> AsImmutableArray<T>(T[]? array)
    {
#if NET8_0_OR_GREATER
        return ImmutableCollectionsMarshal.AsImmutableArray(array);
#else
        return Unsafe.As<T[]?, ImmutableArray<T>>(ref array);
#endif
    }

    public static T[]? AsArray<T>(ImmutableArray<T> array)
    {
#if NET8_0_OR_GREATER
        return ImmutableCollectionsMarshal.AsArray(array);
#else
        return Unsafe.As<ImmutableArray<T>, T[]?>(ref array);
#endif
    }
}

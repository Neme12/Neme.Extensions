using System.Runtime.CompilerServices;

namespace Neme.Extensions.Buffers;

public static class Stackalloc
{
    public const int MaxByteLength = 512;

    public static int MaxLength<T>()
    {
        var elementSize = Unsafe.SizeOf<T>();

        var maxLength = MaxByteLength / elementSize;
        if (maxLength < 1)
            throw new InvalidOperationException($"The maximum byte length is too small to accommodate any elements of type {typeof(T)}.");

        return maxLength;
    }
}

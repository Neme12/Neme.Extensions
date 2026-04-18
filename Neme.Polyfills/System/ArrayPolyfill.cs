namespace System;

public static class ArrayPolyfill
{
    public static int MaxLength =>
#if NET6_0_OR_GREATER
        Array.MaxLength;
#else
        0X7FFFFFC7;
#endif
}

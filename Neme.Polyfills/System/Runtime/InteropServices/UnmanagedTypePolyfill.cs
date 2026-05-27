namespace System.Runtime.InteropServices;

public static class UnmanagedTypePolyfill
{
    public const UnmanagedType LPUTF8Str = (UnmanagedType)48;

#if !(NET47_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
    extension(UnmanagedType)
    {
        public static UnmanagedType LPUTF8Str => (UnmanagedType)48;
    }
#endif
}

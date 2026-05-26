namespace System.Runtime.InteropServices;

#if !(NET47_OR_GREATER || NETCOREAPP)
public static class UnmanagedTypePolyfill
{
    public const UnmanagedType LPUTF8Str = (UnmanagedType)48;

    extension(UnmanagedType)
    {
        public static UnmanagedType LPUTF8Str => (UnmanagedType)48;
    }
}
#endif

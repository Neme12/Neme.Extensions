namespace System.IO;

public static class ValueTaskPolyfill
{
    extension(ValueTask)
    {
        public static ValueTask CompletedTask =>
#if NET5_0_OR_GREATER
            ValueTask.CompletedTask;
#else
            default;
#endif
    }
}

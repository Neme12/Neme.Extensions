using System.Runtime.InteropServices;

namespace Neme.Extensions.Utilities;

[StructLayout(LayoutKind.Auto)]
public struct ValueLazy<T>
{
    private T _value;
    private bool _initialized;
    private object? _lock;

    public T EnsureInitialized(Func<T> valueFactory) =>
        LazyInitializer.EnsureInitialized(ref _value, ref _initialized, ref _lock, valueFactory);
}

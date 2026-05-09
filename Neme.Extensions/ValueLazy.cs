using Roslyn.Utilities;
using System.Runtime.InteropServices;

namespace Neme.Extensions;

[NonCopyable]
[StructLayout(LayoutKind.Auto)]
public struct ValueLazy<T>
{
    private T _value;
    private bool _initialized;
    private object? _lock;

    public T EnsureInitialized(Func<T> valueFactory) =>
        LazyInitializer.EnsureInitialized(ref _value, ref _initialized, ref _lock, valueFactory);
}

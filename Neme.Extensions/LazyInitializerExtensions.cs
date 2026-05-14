using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Neme.Extensions.Contracts;

namespace Neme.Extensions;

public static class LazyInitializerExtensions
{
    public static T EnsureInitialized<T>(
        ref Optional<T> target,
        [NotNullIfNotNull(nameof(syncLock))] ref object? syncLock,
        Func<T> valueFactory)
    {
        Require.ArgumentNotNull(valueFactory);

        if (Volatile.Read(ref Unsafe.AsRef(in Optional.GetHasValueRef(in target))))
            return target.Value;

        return EnsureInitializedCore(ref target, ref syncLock, valueFactory);
    }

    private static T EnsureInitializedCore<T>(
        ref Optional<T> target,
        [NotNull] ref object? syncLock,
        Func<T> valueFactory)
    {
        Debug.AssertNotNull(valueFactory);

        lock (EnsureLockInitialized(ref syncLock))
        {
            if (!Volatile.Read(ref Unsafe.AsRef(in Optional.GetHasValueRef(in target))))
                target = valueFactory();
        }

        return target.Value;
    }

    private static object EnsureLockInitialized([NotNull] ref object? syncLock) =>
        syncLock ??
        Interlocked.CompareExchange(ref syncLock, new object(), null) ??
        syncLock;
}

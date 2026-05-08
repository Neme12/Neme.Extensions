using System.Runtime.CompilerServices;
using Neme.Extensions.Contracts;
using Neme.Extensions.Internal;

namespace Neme.Extensions.InteropServices;

public static class ImmutableCollectionsMarshalExtensions
{
    public static Span<T> AsSpan<T>(ImmutableArray<T>.Builder? builder)
    {
        if (builder is null)
            return default;

        var array = ImmutableArrayBuilderShadow<T>.From(builder)._elements;
        return array.AsSpan(0, builder.Count);
    }

    public static Memory<T> AsMemory<T>(ImmutableArray<T>.Builder? builder)
    {
        if (builder is null)
            return default;

        var array = ImmutableArrayBuilderShadow<T>.From(builder)._elements;
        return array.AsMemory(0, builder.Count);
    }

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    private sealed class ImmutableArrayBuilderShadow<T>
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
#pragma warning disable CS0649 // Field 'ImmutableCollectionsMarshalEx.ImmutableArrayBuilderShadow<T>._count' is never assigned to, and will always have its default value 0
        public T[] _elements = null!;
        public int _count;
#pragma warning restore CS0649 // Field 'ImmutableCollectionsMarshalEx.ImmutableArrayBuilderShadow<T>._count' is never assigned to, and will always have its default value 0

        private ImmutableArrayBuilderShadow()
        {
        }

        public static ImmutableArrayBuilderShadow<T> From(ImmutableArray<T>.Builder builder)
        {
            Assert.True(ShadowTypeUtilities.MatchesShadow(typeof(ImmutableArray<T>.Builder), typeof(ImmutableArrayBuilderShadow<T>)));
            return Unsafe.As<ImmutableArrayBuilderShadow<T>>(builder);
        }
    }
}

using System.Runtime.InteropServices;

namespace Neme.Extensions.InteropServices;

public static class ImmutableCollectionsMarshalExtensions
{
    extension(ImmutableCollectionsMarshal)
    {
        public static Span<T> AsSpan<T>(ImmutableArray<T>.Builder? builder) =>
            ImmutableCollectionsMarshal.AsMemory(builder).Span;
    }
}

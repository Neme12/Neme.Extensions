namespace Neme.Extensions.Ownership;

public static class AsyncMutableDisposable
{
    public static AsyncMutableDisposable<T> Create<T>(T value) where T : IAsyncDisposable? =>
        new(value);
}

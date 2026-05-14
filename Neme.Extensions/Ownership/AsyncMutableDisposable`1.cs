namespace Neme.Extensions.Ownership;

public struct AsyncMutableDisposable<T>(T value) : IAsyncDisposable
    where T : IAsyncDisposable?
{
    private T _value = value;
    private bool _disposed = false;

    public readonly T Value
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _value;
        }
    }

    public async ValueTask SetValueAsync(T newValue)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_value is { } value)
            await value.DisposeAsync();

        _value = newValue;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_value is { } value)
                await value.DisposeAsync();

            _disposed = true;
        }
    }
}

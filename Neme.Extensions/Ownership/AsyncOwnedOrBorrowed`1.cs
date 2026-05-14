namespace Neme.Extensions.Ownership;

public struct AsyncOwnedOrBorrowed<T>(T value, bool ownsValue = true) : IAsyncDisposable
    where T : IAsyncDisposable
{
    private T _value = value;
    private bool _ownsValue = ownsValue;

    public readonly T Value =>
        _value;

    public readonly bool OwnsValue =>
        _ownsValue;

    public async ValueTask SetValueAsync(T newValue, bool ownsNewValue = true)
    {
        if (_ownsValue)
            await _value.DisposeAsync();

        _value = newValue;
        _ownsValue = ownsNewValue;
    }

    public T Move()
    {
        if (!_ownsValue)
            throw new InvalidOperationException("Cannot move a value that is not owned.");

        _ownsValue = false;
        return _value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_ownsValue)
        {
            await _value.DisposeAsync();
            _ownsValue = false;
        }
    }
}

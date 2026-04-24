namespace Neme.Extensions.Ownership;

public struct AsyncOwnedOrBorrowed<T>(T value, bool ownsValue = true) : IAsyncDisposable
    where T : IAsyncDisposable
{
    private readonly T _value = value;
    private bool _ownsValue = ownsValue;

    public readonly T Value =>
        _value;

    public readonly bool OwnsValue =>
        _ownsValue;

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

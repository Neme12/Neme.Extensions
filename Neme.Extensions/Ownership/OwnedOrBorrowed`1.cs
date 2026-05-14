namespace Neme.Extensions.Ownership;

public struct OwnedOrBorrowed<T>(T value, bool ownsValue = true) : IDisposable
    where T : IDisposable
{
    private T _value = value;
    private bool _ownsValue = ownsValue;

    public readonly T Value =>
        _value;

    public readonly bool OwnsValue =>
        _ownsValue;

    public void SetValue(T newValue, bool ownsNewValue = true)
    {
        if (_ownsValue)
            _value.Dispose();

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

    public void Dispose()
    {
        if (_ownsValue)
        {
            _value.Dispose();
            _ownsValue = false;
        }
    }
}

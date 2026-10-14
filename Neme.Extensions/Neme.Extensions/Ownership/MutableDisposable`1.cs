namespace Neme.Extensions.Ownership;

public struct MutableDisposable<T>(T value) : IDisposable
    where T : IDisposable?
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

    public void SetValue(T newValue)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _value?.Dispose();
        _value = newValue;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _value?.Dispose();
            _disposed = true;
        }
    }
}

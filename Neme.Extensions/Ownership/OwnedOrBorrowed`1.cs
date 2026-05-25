namespace Neme.Extensions.Ownership;

public struct OwnedOrBorrowed<T>(T value, bool ownsValue = true) : IDisposable
    where T : IDisposable?
{
    private T _value = value;
    private State _state = ownsValue ? State.Owned : State.Borrowed;

    public readonly T Value
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);
            return _value;
        }
    }

    public readonly bool OwnsValue
    {
        get
        {
            ObjectDisposedException.ThrowIf(_state == State.Disposed, this);
            return _state == State.Owned;
        }
    }

    public T Move()
    {
        ObjectDisposedException.ThrowIf(_state == State.Disposed, this);

        if (_state != State.Owned)
            ThrowNotOwned();

        _state = State.Borrowed;
        return _value;

        static void ThrowNotOwned() =>
            throw new InvalidOperationException("Cannot move a value that is not owned.");
    }

    public void Dispose()
    {
        if (_state != State.Disposed)
        {
            if (_state == State.Owned)
                _value?.Dispose();

            _state = State.Disposed;
        }
    }

    private enum State : byte
    {
        Owned,
        Borrowed,
        Disposed,
    }
}

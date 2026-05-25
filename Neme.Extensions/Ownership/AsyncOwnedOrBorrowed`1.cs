namespace Neme.Extensions.Ownership;

public struct AsyncOwnedOrBorrowed<T>(T value, bool ownsValue = true) : IAsyncDisposable
    where T : IAsyncDisposable?
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

    public async ValueTask DisposeAsync()
    {
        if (_state != State.Disposed)
        {
            if (_state == State.Owned && _value is { } value)
                await value.DisposeAsync();

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

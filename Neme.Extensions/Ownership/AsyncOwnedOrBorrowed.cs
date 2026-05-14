namespace Neme.Extensions.Ownership;

public static class AsyncOwnedOrBorrowed
{
    public static AsyncOwnedOrBorrowed<T> Create<T>(T value, bool ownsValue = true) where T : IAsyncDisposable? =>
        new(value, ownsValue);

    public static AsyncOwnedOrBorrowed<T> CreateOwned<T>(T value) where T : IAsyncDisposable? =>
        new(value, ownsValue: true);

    public static AsyncOwnedOrBorrowed<T> CreateBorrowed<T>(T value) where T : IAsyncDisposable? =>
        new(value, ownsValue: false);
}

namespace Neme.Extensions.Ownership;

public static class MutableDisposable
{
    public static MutableDisposable<T> Create<T>(T value) where T : IDisposable? =>
        new(value);
}

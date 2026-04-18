namespace Neme.Extensions.Utilities;

public static class OwnedOrBorrowed
{
    public static OwnedOrBorrowed<T> Create<T>(T value, bool ownsValue = true) where T : IDisposable =>
        new(value, ownsValue);

    public static OwnedOrBorrowed<T> CreateOwned<T>(T value) where T : IDisposable =>
        new(value, ownsValue: true);

    public static OwnedOrBorrowed<T> CreateBorrowed<T>(T value) where T : IDisposable =>
        new(value, ownsValue: false);
}

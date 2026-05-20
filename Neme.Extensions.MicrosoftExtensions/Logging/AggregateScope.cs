namespace Neme.Extensions.MicrosoftExtensions.Logging;

internal sealed class AggregateScope : IDisposable
{
    private ImmutableArray<IDisposable?> _scopes;

    public AggregateScope(ImmutableArray<IDisposable?> scopes)
    {
        _scopes = scopes;
    }

    public void Dispose()
    {
        if (!_scopes.IsDefault)
        {
            foreach (var scope in _scopes)
                scope?.Dispose();

            _scopes = default;
        }
    }
}

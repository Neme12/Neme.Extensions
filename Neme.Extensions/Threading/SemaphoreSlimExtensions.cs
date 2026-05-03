namespace Neme.Extensions.Threading;

public static class SemaphoreSlimExtensions
{
    extension(SemaphoreSlim semaphore)
    {
        public async Task<Scope> WaitScopeAsync(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            return new Scope(semaphore);
        }
    }

    public struct Scope : IDisposable
    {
        private SemaphoreSlim _semaphore;

        internal Scope(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            if (_semaphore is not null)
            {
                _semaphore.Release();
                _semaphore = null!;
            }
        }
    }
}

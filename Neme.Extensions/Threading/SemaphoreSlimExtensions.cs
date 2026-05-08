using Roslyn.Utilities;

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

    [NonCopyable]
    public struct Scope : IDisposable
    {
#pragma warning disable RS0040
        private SemaphoreSlim _semaphore;
#pragma warning restore RS0040

        internal Scope(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public readonly SemaphoreSlim Semaphore =>
            _semaphore;

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

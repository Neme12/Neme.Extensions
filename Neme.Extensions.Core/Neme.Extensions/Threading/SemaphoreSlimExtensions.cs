using Roslyn.Utilities;
using System.Runtime.Versioning;

namespace Neme.Extensions.Threading;

public static class SemaphoreSlimExtensions
{
    extension(SemaphoreSlim semaphore)
    {
        [UnsupportedOSPlatform("browser")]
        public Scope WaitScope(CancellationToken cancellationToken)
        {
            semaphore.Wait(cancellationToken);
            return new Scope(semaphore);
        }

        public async Task<Scope> WaitScopeAsync(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            return new Scope(semaphore);
        }

        [UnsupportedOSPlatform("browser")]
        public async ValueTask<Scope> WaitScopeMaybeAsync<TAsync>(CancellationToken cancellationToken)
            where TAsync : struct, IAsyncState
        {
            if (typeof(TAsync) == typeof(IAsyncState.Async))
            {
                await semaphore.WaitAsync(cancellationToken);
            }
            else
            {
                semaphore.Wait(cancellationToken);
            }

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

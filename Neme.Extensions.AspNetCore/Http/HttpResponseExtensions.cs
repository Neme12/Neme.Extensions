using Microsoft.AspNetCore.Http;
using Roslyn.Utilities;

namespace Microsoft.Extensions.AspNetCore.Http;

public static class HttpResponseExtensions
{
    extension(HttpResponse response)
    {
        public async Task<Scope> StartScopeAsync(CancellationToken cancellationToken = default)
        {
            await response.StartAsync(cancellationToken).ConfigureAwait(false);
            return new Scope(response);
        }
    }

    [NonCopyable]
    public struct Scope : IAsyncDisposable
    {
#pragma warning disable RS0040
        private HttpResponse _response;
#pragma warning restore RS0040

        internal Scope(HttpResponse response)
        {
            _response = response;
        }

        public readonly HttpResponse Response =>
            _response;

        public async ValueTask DisposeAsync()
        {
            if (_response is not null)
            {
                var response = _response;
                _response = null!;
                await response.CompleteAsync().ConfigureAwait(false);
            }
        }
    }
}

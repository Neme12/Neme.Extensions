using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.AspNetCore.Http;

public static class HttpResponseExtensions
{
    extension(HttpResponse response)
    {
        public async Task<Scope> StartScopeAsync(CancellationToken cancellationToken = default)
        {
            await response.StartAsync(cancellationToken);
            return new Scope(response);
        }
    }

    public struct Scope : IAsyncDisposable
    {
        private HttpResponse _response;

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
                _response = null!;
                await _response.CompleteAsync();
            }
        }
    }
}

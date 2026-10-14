namespace Neme.Extensions;

public static class UriExtensions
{
    extension(Uri uri)
    {
        public bool IsWebScheme
        {
            get
            {
                var scheme = uri.Scheme;

                return
                    scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                    scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}

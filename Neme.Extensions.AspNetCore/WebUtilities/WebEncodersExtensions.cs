using Microsoft.AspNetCore.WebUtilities;
using System.Buffers.Text;
using System.Diagnostics;

namespace Neme.Extensions.AspNetCore.WebUtilities;

public static class WebEncodersExtensions
{
    extension(WebEncoders)
    {
        public static string Base64UrlEncodeGuid(Guid input)
        {
            Span<byte> bytes = stackalloc byte[Guid.ByteLength];
            Trace.Assert(input.TryWriteBytes(bytes));

#if NET9_0_OR_GREATER
            return Base64Url.EncodeToString(bytes);
#else
            return WebEncoders.Base64UrlEncode(bytes);
#endif
        }

        public static Guid Base64UrlDecodeGuid(ReadOnlySpan<char> input)
        {
#if NET9_0_OR_GREATER
            Span<byte> bytes = stackalloc byte[Guid.ByteLength];

            var charsWritten = Base64Url.DecodeFromChars(input, bytes);
            if (charsWritten != Guid.ByteLength)
                throw new FormatException("The input string is not a valid Base64Url-encoded UUID.");

            return new Guid(bytes);
#else
            var bytes = WebEncoders.Base64UrlDecode(input.ToString());
            return new Guid(bytes);
#endif
        }

        public static Guid Base64UrlDecodeGuid(string input)
        {
            // Only in older .NET versions, where we have to convert the ReadOnlySpan<char> to a string,
            // do we gain benefit by this overload so that the string isn't converted to a ReadOnlySpan<char>
            // at the call site, and then back to a new string.
#if NET9_0_OR_GREATER
            return Base64UrlDecodeGuid(input.AsSpan());
#else
            var bytes = WebEncoders.Base64UrlDecode(input);
            return new Guid(bytes);
#endif
        }
    }
}

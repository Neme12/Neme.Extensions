using Neme.Extensions.Buffers;
using System.Buffers;
using System.Text;

namespace Neme.Extensions.IO;

public static class TextReaderExtensions
{
    extension(TextReader textReader)
    {
        public string ReadToEnd(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stringBuilder = new StringBuilder(4096);

            using (var chars = ArrayPool<char>.Shared.RentLease(4096))
            {
                int len;

                while ((len = textReader.Read(chars.Array, 0, chars.Length)) != 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    stringBuilder.Append(chars.Array, 0, len);
                }
            }

            return stringBuilder.ToString();
        }
    }
}

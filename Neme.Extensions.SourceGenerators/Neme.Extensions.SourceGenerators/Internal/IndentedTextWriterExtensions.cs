using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.Internal;

internal static class IndentedTextWriterExtensions
{
    extension(IndentedTextWriter)
    {
        public static IndentedTextWriter CreateNew()
        {
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);

            // IndentedTextWriter disposal disposes the underlying StringWriter, so we don't dispose it.
            return new IndentedTextWriter(stringWriter);
        }
    }
}

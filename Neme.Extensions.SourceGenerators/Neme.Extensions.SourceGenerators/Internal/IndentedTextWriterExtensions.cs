using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;

namespace Neme.Extensions.SourceGenerators.Internal;

internal static class IndentedTextWriterExtensions
{
    extension(IndentedTextWriter writer)
    {
        public static IndentedTextWriter CreateNew(int initialIndent,int initialCapacity = 0)
        {
            var stringWriter = new StringWriter(new StringBuilder(initialCapacity), CultureInfo.InvariantCulture);

            // IndentedTextWriter disposal disposes the underlying StringWriter, so we don't dispose it.
            return new IndentedTextWriter(stringWriter) { Indent = initialIndent };
        }

        public IndentScope SetIndent(int indent)
        {
            var originalIndent = writer.Indent;
            writer.Indent = indent;
            return new IndentScope(writer, originalIndent);
        }

        public IndentScope AddIndent()
        {
            var originalIndent = writer.Indent;
            writer.Indent += 1;
            return new IndentScope(writer, originalIndent);
        }
    }

    public struct IndentScope : IDisposable
    {
        private IndentedTextWriter _writer;
        private readonly int _originalIndent;

        public IndentScope(IndentedTextWriter writer, int originalIndent)
        {
            _writer = writer;
            _originalIndent = originalIndent;
        }

        public void Dispose()
        {
            if (_writer is null)
                return;

            _writer.Indent = _originalIndent;
            _writer = null!;
        }
    }
}

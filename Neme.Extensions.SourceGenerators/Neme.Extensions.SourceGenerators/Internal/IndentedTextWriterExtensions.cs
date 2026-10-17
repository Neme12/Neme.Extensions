using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.Internal;

internal static class IndentedTextWriterExtensions
{
    extension(IndentedTextWriter writer)
    {
        public static IndentedTextWriter CreateNew(int initialIndent)
        {
            var stringWriter = new StringWriter(CultureInfo.InvariantCulture);

            // IndentedTextWriter disposal disposes the underlying StringWriter, so we don't dispose it.
            return new IndentedTextWriter(stringWriter) { Indent = initialIndent };
        }

        public IndentScope WithIndent(int indent)
        {
            var originalIndent = writer.Indent;
            writer.Indent = indent;
            return new IndentScope(writer, originalIndent);
        }

        public IndentScope WithAddedIndent()
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

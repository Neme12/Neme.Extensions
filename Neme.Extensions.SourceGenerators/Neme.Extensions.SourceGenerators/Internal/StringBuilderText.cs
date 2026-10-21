using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Neme.Extensions.SourceGenerators.Internal;

internal sealed class StringBuilderText : SourceText
{
    private readonly StringBuilder _builder;
    private readonly Encoding? _encoding;

    public StringBuilderText(StringBuilder builder, Encoding? encoding, SourceHashAlgorithm checksumAlgorithm)
         : base(checksumAlgorithm: checksumAlgorithm)
    {
        Debug.Assert(builder is not null);

        _builder = builder!;
        _encoding = encoding;
    }

    public override Encoding? Encoding =>
        _encoding;

    internal StringBuilder Builder =>
        _builder;

    public override int Length =>
        _builder.Length;

    public override char this[int position]
    {
        get
        {
            if (position < 0 || position >= _builder.Length)
                Throw();

            return _builder[position];

            [DoesNotReturn]
            static void Throw() =>
                throw new ArgumentOutOfRangeException(nameof(position));
        }
    }

    public override string ToString(TextSpan span)
    {
        if (span.End > _builder.Length)
            Throw();

        return _builder.ToString(span.Start, span.Length);

        [DoesNotReturn]
        static void Throw() =>
            throw new ArgumentOutOfRangeException(nameof(span));
    }

    public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
        _builder.CopyTo(sourceIndex, destination, destinationIndex, count);
}

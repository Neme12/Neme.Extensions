using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumIncludesAnyGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly string _enumTypeName = generationContext.EnumTypeName;

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine($"public bool IncludesAny({_enumTypeName} flag) =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("(@enum & flag) != 0;");
            textWriter.WriteLine();
        }

        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine($"public bool IncludesAny(params global::System.ReadOnlySpan<{_enumTypeName}> flags)");
        textWriter.WriteLine('{');

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("foreach (var flag in flags)");
            textWriter.WriteLine('{');

            using (textWriter.AddIndent())
            {
                textWriter.WriteLine("if (IncludesAny(@enum, flag))");

                using (textWriter.AddIndent())
                    textWriter.WriteLine("return true;");
            }

            textWriter.WriteLine('}');
            textWriter.WriteLine();
            textWriter.WriteLine("return false;");
        }

        textWriter.WriteLine('}');

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}


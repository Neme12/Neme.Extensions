using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumIncludesAllGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

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

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        textWriter.WriteLine($"public bool IncludesAll({enumTypeName} flag) =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("(@enum & flag) == flag;");
            textWriter.WriteLine();
        }

        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine($"public bool IncludesAll(params global::System.ReadOnlySpan<{enumTypeName}> flags)");
        textWriter.WriteLine('{');

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("foreach (var flag in flags)");
            textWriter.WriteLine('{');

            using (textWriter.AddIndent())
            {
                textWriter.WriteLine("if (!IncludesAll(@enum, flag))");

                using (textWriter.AddIndent())
                    textWriter.WriteLine("return false;");
            }

            textWriter.WriteLine('}');
            textWriter.WriteLine();
            textWriter.WriteLine("return true;");
        }

        textWriter.WriteLine('}');

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}


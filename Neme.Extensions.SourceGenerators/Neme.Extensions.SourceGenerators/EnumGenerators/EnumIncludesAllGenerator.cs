using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumIncludesAllGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public string? GetTypeMemberSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        return null;
    }

    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
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

            textWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return textWriter.InnerWriter.ToString();
        }
    }
}


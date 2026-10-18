using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticAllGenerator(INamedTypeSymbol enumType, Compilation compilation) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly Compilation _compilation = compilation;

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

        if (GeneratorUtilities.HasConflictingFieldOrProperty(_compilation, _enumType, "All", cancellationToken))
            return false;

        var enumMembers = GeneratorUtilities.GetEnumMembers(_enumType, cancellationToken);

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var allParts = new string[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allParts[i] = Invariant($"""{enumTypeName}.{enumMembers[i].Name}""");
        }

        textWriter.WriteLine($"public static {enumTypeName} All =>");

        using (textWriter.AddIndent())
        {
            if (allParts.Length == 0)
                textWriter.WriteLine("default;");
            else
            {
                for (var i = 0; i < allParts.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var suffix = i == allParts.Length - 1 ? ";" : " |";
                    textWriter.WriteLine($"{allParts[i]}{suffix}");
                }
            }
        }

        return true;
    }
}

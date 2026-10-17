using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticAllGenerator(INamedTypeSymbol enumType, Compilation compilation) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly Compilation _compilation = compilation;

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

        if (GeneratorUtilities.HasConflictingFieldOrProperty(_compilation, _enumType, "All", cancellationToken))
            return null;

        var enumMembers = GeneratorUtilities.GetEnumMembers(_enumType, cancellationToken);

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var allParts = new string[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allParts[i] = $"{enumTypeName}.{enumMembers[i].Name}";
        }

        using (var textWriter = IndentedTextWriter.CreateNew())
        {
            textWriter.Indent = baselineIndent;

            if (allParts.Length == 0)
            {
                textWriter.WriteLine($"public static {enumTypeName} All =>");
                textWriter.Indent = baselineIndent + 1;
                textWriter.WriteLine("default;");
            }
            else
            {
                textWriter.WriteLine($"public static {enumTypeName} All =>");
                textWriter.Indent = baselineIndent + 1;

                for (var i = 0; i < allParts.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var suffix = i == allParts.Length - 1 ? ";" : " |";
                    textWriter.WriteLine($"{allParts[i]}{suffix}");
                }
            }

            return textWriter.InnerWriter.ToString();
        }
    }
}

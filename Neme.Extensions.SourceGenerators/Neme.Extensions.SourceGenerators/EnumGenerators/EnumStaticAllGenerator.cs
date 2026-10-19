using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticAllGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly Compilation _compilation = generationContext.Compilation;
    private readonly INamedTypeSymbol _enumType = generationContext.EnumType;
    private readonly string _enumTypeName = generationContext.EnumTypeName;
    private readonly ImmutableArray<IFieldSymbol> _enumMembers = generationContext.EnumMembers;

    public bool EmitTypeSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool EmitExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (GeneratorUtilities.HasConflictingFieldOrProperty(_compilation, _enumType, "All", cancellationToken))
            return false;

        var allParts = new string[_enumMembers.Length];
        for (var i = 0; i < _enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allParts[i] = Invariant($"""{_enumTypeName}.{_enumMembers[i].Name}""");
        }

        textWriter.WriteLine($"public static {_enumTypeName} All =>");

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

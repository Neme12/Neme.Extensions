using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticUnderlyingValuesGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly string _enumTypeName = generationContext.EnumTypeName;
    private readonly string? _underlyingTypeName = generationContext.UnderlyingTypeName;
    private readonly ImmutableArray<IFieldSymbol> _enumMembers = generationContext.ConstantEnumMembers;

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

        if (_underlyingTypeName is null)
            return false;

        textWriter.WriteLine($"public static global::System.ReadOnlySpan<{_underlyingTypeName}> UnderlyingValues =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("[");

            using (textWriter.AddIndent())
            {
                foreach (var member in _enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"({_underlyingTypeName}){_enumTypeName}.{member.Name},");
                }
            }

            textWriter.WriteLine("];");
        }

        return true;
    }
}

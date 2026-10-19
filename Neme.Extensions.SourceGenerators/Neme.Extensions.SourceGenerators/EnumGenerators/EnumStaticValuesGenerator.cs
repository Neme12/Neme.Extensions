using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticValuesGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly string _enumTypeName = generationContext.EnumTypeName;
    private readonly ImmutableArray<IFieldSymbol> _enumMembers = generationContext.ConstantEnumMembers;

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

        textWriter.WriteLine($"public static global::System.ReadOnlySpan<{_enumTypeName}> Values =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("[");

            using (textWriter.AddIndent())
            {
                foreach (var member in _enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"{_enumTypeName}.{member.Name},");
                }
            }

            textWriter.WriteLine("];");
        }

        return true;
    }
}

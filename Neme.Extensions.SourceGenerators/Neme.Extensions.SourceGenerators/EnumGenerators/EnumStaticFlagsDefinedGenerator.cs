using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticFlagsDefinedGenerator(EnumGenerationContext generationContext) : IEnumGenerator
{
    private readonly string _enumTypeName = generationContext.EnumTypeName;
    private readonly string? _underlyingTypeName = generationContext.UnderlyingTypeName;
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

        if (_underlyingTypeName is null)
            return false;

        var allFlagsParts = new string[_enumMembers.Length];
        for (var i = 0; i < _enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();
            allFlagsParts[i] = Invariant($"""({_underlyingTypeName}){_enumTypeName}.{_enumMembers[i].Name}""");
        }

        textWriter.WriteLine($"public static bool FlagsDefined({_enumTypeName} value)");
        textWriter.WriteLine("{");

        using (textWriter.AddIndent())
        {
            if (allFlagsParts.Length == 0)
                textWriter.WriteLine($"{_underlyingTypeName} allFlags = default;");
            else
                textWriter.WriteLine($"{_underlyingTypeName} allFlags =");

            if (allFlagsParts.Length > 0)
            {
                using (textWriter.AddIndent())
                {
                    for (var i = 0; i < allFlagsParts.Length; ++i)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var suffix = i == allFlagsParts.Length - 1 ? ";" : " |";
                        textWriter.WriteLine($"{allFlagsParts[i]}{suffix}");
                    }
                }
            }

            textWriter.WriteLine();
            textWriter.WriteLine($"return ((({_underlyingTypeName})value) | allFlags) == allFlags;");
        }

        textWriter.WriteLine("}");

        return true;
    }
}

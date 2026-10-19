using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticIsDefinedGenerator(EnumGenerationContext generationContext) : IEnumGenerator
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

        var seenValues = new HashSet<object>();
        var definedPatterns = new List<string>(_enumMembers.Length);
        foreach (var member in _enumMembers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!seenValues.Add(member.ConstantValue!))
                continue;

            definedPatterns.Add(Invariant($"""{_enumTypeName}.{member.Name}"""));
        }

        if (definedPatterns.Count == 0)
            textWriter.WriteLine($"public static bool IsDefined({_enumTypeName} value) => false;");
        else
        {
            textWriter.WriteLine($"public static bool IsDefined({_enumTypeName} value) =>");

            using (textWriter.AddIndent())
            {
                textWriter.WriteLine("value is");

                using (textWriter.AddIndent())
                {
                    for (var i = 0; i < definedPatterns.Count; ++i)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var suffix = i == definedPatterns.Count - 1 ? ";" : " or";
                        textWriter.WriteLine($"{definedPatterns[i]}{suffix}");
                    }
                }
            }
        }

        return true;
    }
}

using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticIsDefinedGenerator(INamedTypeSymbol enumType, string enumTypeName) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly string _enumTypeName = enumTypeName;

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

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        var seenValues = new HashSet<object>();
        var definedPatterns = new List<string>(enumMembers.Length);
        foreach (var member in enumMembers)
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

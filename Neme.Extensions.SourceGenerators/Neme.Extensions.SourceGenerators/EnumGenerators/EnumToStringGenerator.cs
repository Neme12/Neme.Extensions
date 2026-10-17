using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumToStringGenerator(INamedTypeSymbol enumType) : IEnumGenerator
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

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        using (var switchWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            {
                switchWriter.WriteLine("public string ToStringGenerated() =>");

                using (switchWriter.WithIndent(baselineIndent + 1))
                {
                    switchWriter.WriteLine("@enum switch");
                    switchWriter.WriteLine("{");

                    using (switchWriter.WithIndent(baselineIndent + 2))
                    {
                        var seenValues = new HashSet<object>();

                        foreach (var member in enumMembers)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var constantValue = member.ConstantValue;
                            if (constantValue is null || !seenValues.Add(constantValue))
                                continue;

                            switchWriter.WriteLine($"{enumTypeName}.{member.Name} => nameof({enumTypeName}.{member.Name}),");
                        }

                        switchWriter.WriteLine("_ => @enum.ToString(),");
                    }

                    switchWriter.WriteLine("};");
                }
            }

            switchWriter.Flush();

            cancellationToken.ThrowIfCancellationRequested();

            return switchWriter.InnerWriter.ToString();
        }
    }
}

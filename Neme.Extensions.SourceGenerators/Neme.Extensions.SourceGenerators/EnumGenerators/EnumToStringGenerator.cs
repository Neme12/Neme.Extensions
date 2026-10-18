using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumToStringGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

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

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine("public string ToStringGenerated() =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("@enum switch");
            textWriter.WriteLine("{");

            using (textWriter.AddIndent())
            {
                var seenValues = new HashSet<object>();

                foreach (var member in enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var constantValue = member.ConstantValue;
                    if (constantValue is null || !seenValues.Add(constantValue))
                        continue;

                    textWriter.WriteLine($"{enumTypeName}.{member.Name} => nameof({enumTypeName}.{member.Name}),");
                }

                textWriter.WriteLine("_ => @enum.ToString(),");
            }

            textWriter.WriteLine("};");
        }

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}

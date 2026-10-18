using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumNameGenerator(INamedTypeSymbol enumType, string enumTypeName) : IEnumGenerator
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

        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine("public string? Name =>");

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

                    textWriter.WriteLine($"{_enumTypeName}.{member.Name} => nameof({_enumTypeName}.{member.Name}),");
                }

                textWriter.WriteLine("_ => global::System.Enum.GetName(@enum),");
            }

            textWriter.WriteLine("};");
        }

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }
}

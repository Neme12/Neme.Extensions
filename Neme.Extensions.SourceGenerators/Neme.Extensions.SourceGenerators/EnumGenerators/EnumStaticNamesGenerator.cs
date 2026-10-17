using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticNamesGenerator(INamedTypeSymbol enumType) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;

    public string? GetTypeMemberSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = _enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            textWriter.WriteLine("private static string[] AllNames");
            textWriter.WriteLine("{");

            using (textWriter.WithIndent(baselineIndent + 1))
            {
                textWriter.WriteLine("get => global::System.Threading.LazyInitializer.EnsureInitialized(ref field, static () =>");
                textWriter.WriteLine("[");

                using (textWriter.WithIndent(baselineIndent + 2))
                {
                    foreach (var member in enumMembers)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        textWriter.WriteLine($"nameof({enumTypeName}.{member.Name}),");
                    }
                }

                textWriter.WriteLine("]!);");
            }

            textWriter.WriteLine("}");

            return textWriter.InnerWriter.ToString();
        }
    }

    public string? GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var textWriter = IndentedTextWriter.CreateNew(baselineIndent))
        {
            textWriter.WriteLine("public static global::System.Collections.Immutable.ImmutableArray<string> Names =>");

            using (textWriter.WithIndent(baselineIndent + 1))
                textWriter.WriteLine("global::System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(AllNames);");

            return textWriter.InnerWriter.ToString();
        }
    }
}

using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticNamesGenerator(INamedTypeSymbol enumType, string enumTypeName) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly string _enumTypeName = enumTypeName;

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumMembers = GeneratorUtilities.GetEnumMembersWithConstantValue(_enumType, cancellationToken);

        textWriter.WriteLine("private static string[] AllNames");
        textWriter.WriteLine("{");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("get => global::System.Threading.LazyInitializer.EnsureInitialized(ref field, static () =>");
            textWriter.WriteLine("[");

            using (textWriter.AddIndent())
            {
                foreach (var member in enumMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"nameof({_enumTypeName}.{member.Name}),");
                }
            }

            textWriter.WriteLine("]!);");
        }

        textWriter.WriteLine("}");
        return true;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine("public static global::System.Collections.Immutable.ImmutableArray<string> Names =>");

        using (textWriter.AddIndent())
            textWriter.WriteLine("global::System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(AllNames);");

        return true;
    }
}

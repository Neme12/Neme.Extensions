using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Globalization;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal static class EnumStaticNamesGenerator
{
    public static string GetTypeMemberSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var enumMembers = enumType
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(static field => field.HasConstantValue)
            .ToImmutableArray();

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine("private static string[] AllNames");
            textWriter.WriteLine("{");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("get => global::System.Threading.LazyInitializer.EnsureInitialized(ref field, static () =>");
            textWriter.WriteLine("[");
            textWriter.Indent = baselineIndent + 2;

            foreach (var member in enumMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                textWriter.WriteLine($"nameof({enumTypeName}.{member.Name}),");
            }

            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("]!);");
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine("}");

            return textWriter.InnerWriter.ToString();
        }
    }

    public static string GetExtensionSource(
        SourceProductionContext context,
        int baselineIndent,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
        using (var textWriter = new IndentedTextWriter(stringWriter))
        {
            textWriter.Indent = baselineIndent;
            textWriter.WriteLine("public static global::System.Collections.Immutable.ImmutableArray<string> Names =>");
            textWriter.Indent = baselineIndent + 1;
            textWriter.WriteLine("global::System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(AllNames);");

            return textWriter.InnerWriter.ToString();
        }
    }
}

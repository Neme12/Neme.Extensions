using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.EnumGenerators;
using System.Text;
using static System.FormattableString;

namespace Neme.Extensions.SourceGenerators;

#pragma warning disable RS1038 // The package is installed
[Generator(LanguageNames.CSharp)]
#pragma warning restore RS1038
public sealed class EnumSourceGenerator : IIncrementalGenerator
{
    private static readonly SymbolDisplayFormat s_fullyQualifiedMinimalFormat = new(
        SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        SymbolDisplayGenericsOptions.None,
        SymbolDisplayMemberOptions.IncludeContainingType);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is EnumDeclarationSyntax,
                static (syntaxContext, cancellationToken) => GetEnumSymbol(syntaxContext, cancellationToken))
            .Where(static symbol => symbol is INamedTypeSymbol enumSymbol && IsAccessibleFromNamespaceLevel(enumSymbol));

        var configuredEnumTypes = context.AnalyzerConfigOptionsProvider.Select(
            static (optionsProvider, _) =>
                optionsProvider.GlobalOptions.TryGetValue("build_property.EnumExtensionTypes", out var enumExtensionTypes)
                    ? enumExtensionTypes
                    : null);

        var generationInput = context.CompilationProvider
            .Combine(enumSymbols.Collect())
            .Combine(configuredEnumTypes);

        context.RegisterSourceOutput(
            generationInput,
            static (sourceContext, value) =>
            {
                var compilation = value.Left.Left;
                var sourceEnums = value.Left.Right;
                var configuredEnumTypes = value.Right;

                var processedEnumTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

                foreach (var enumType in sourceEnums)
                {
                    sourceContext.CancellationToken.ThrowIfCancellationRequested();

                    if (enumType is null || !processedEnumTypes.Add(enumType))
                        continue;

                    if (!compilation.IsSymbolAccessibleWithin(enumType, compilation.Assembly))
                        continue;

                    EmitEnumSource(sourceContext, enumType, compilation, sourceContext.CancellationToken);
                }

                if (string.IsNullOrWhiteSpace(configuredEnumTypes))
                    return;

                foreach (var enumTypeMetadataName in configuredEnumTypes.Split(';'))
                {
                    sourceContext.CancellationToken.ThrowIfCancellationRequested();

                    var normalizedMetadataName = enumTypeMetadataName.Trim();
                    if (normalizedMetadataName.Length == 0)
                        continue;

                    if (compilation.GetTypeByMetadataName(normalizedMetadataName) is not INamedTypeSymbol enumType)
                        continue;

                    if (enumType.TypeKind != TypeKind.Enum)
                        continue;

                    if (!IsAccessibleFromNamespaceLevel(enumType))
                        continue;

                    if (!compilation.IsSymbolAccessibleWithin(enumType, compilation.Assembly))
                        continue;

                    if (!processedEnumTypes.Add(enumType))
                        continue;

                    EmitEnumSource(sourceContext, enumType, compilation, sourceContext.CancellationToken);
                }
            });
    }

    private static INamedTypeSymbol? GetEnumSymbol(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumDeclaration = (EnumDeclarationSyntax)context.Node;
        return context.SemanticModel.GetDeclaredSymbol(enumDeclaration) as INamedTypeSymbol;
    }

    private static void EmitEnumSource(
        SourceProductionContext context,
        INamedTypeSymbol enumType,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        Span<string?> typeSources =
            [
                EnumNamesGenerator.GetTypeMemberSource(context, 1, enumType, cancellationToken)
            ];

        ReadOnlySpan<string?> flagExtensionSources = IsFlagsEnum(compilation, enumType)
            ? [
                EnumIncludesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken)!,
                EnumIncludesAllGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumIncludesAnyGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
            ]
            : [];

        Span<string?> extensionSources =
            [
                EnumNameGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumNamesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumValueGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                ..flagExtensionSources,
            ];

        EmitEnumExtension(
            context,
            typeSources.RemoveNullValuesInPlace(),
            extensionSources.RemoveNullValuesInPlace(),
            enumType,
            cancellationToken);
    }

    private static bool IsFlagsEnum(Compilation compilation, INamedTypeSymbol enumType)
    {
        var flagsAttributeType = compilation.GetTypeByMetadataName("System.FlagsAttribute");
        if (flagsAttributeType is null)
            return false;

        foreach (var attribute in enumType.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, flagsAttributeType))
                return true;
        }

        return false;
    }

    private static void EmitEnumExtension(
        SourceProductionContext context,
        ReadOnlySpan<string> typeSources,
        ReadOnlySpan<string> extensionSources,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var typeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var namespaceDeclaration = enumType.ContainingNamespace.IsGlobalNamespace
            ? ""
            : Invariant($"""

                namespace {enumType.ContainingNamespace.ToDisplayString()};

                """);

        var extensionClassName = GetExtensionClassName(enumType);

        cancellationToken.ThrowIfCancellationRequested();

        var result =
            // lang=c#
            $$"""
            // <auto-generated/>
            {{namespaceDeclaration}}
            #nullable enable

            [global::System.CodeDom.Compiler.GeneratedCode("Neme.Extensions.SourceGenerators", "0.0.0.0")]
            internal static class {{extensionClassName}}
            {
            {{string.Join(Environment.NewLine, typeSources)}}

                extension({{typeName}} value)
                {
            {{string.Join(Environment.NewLine, extensionSources)}}
                }
            }

            """;

        cancellationToken.ThrowIfCancellationRequested();

        context.AddSource(GetFileName(enumType), SourceText.From(result, Encoding.UTF8));

    }

    private static string GetExtensionClassName(INamedTypeSymbol enumType)
    {
        var containingTypes = new Stack<string>();
        for (var containingType = enumType.ContainingType; containingType is not null; containingType = containingType.ContainingType)
            containingTypes.Push(containingType.Name);

        var typePrefix = containingTypes.Count == 0
            ? ""
            : Invariant($"{string.Join("_", containingTypes)}_");
        return Invariant($"{typePrefix}{enumType.Name}Extensions");
    }

    private static bool IsAccessibleFromNamespaceLevel(INamedTypeSymbol enumType)
    {
        if (!IsNamespaceVisible(enumType.DeclaredAccessibility))
            return false;

        for (var containingType = enumType.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            if (!IsNamespaceVisible(containingType.DeclaredAccessibility))
                return false;
        }

        return true;
    }

    private static bool IsNamespaceVisible(Accessibility accessibility) =>
        accessibility is Accessibility.Public or Accessibility.Internal;

    private static string GetFileName(INamedTypeSymbol enumType)
    {
        var fullyQualifiedName = enumType.ToDisplayString(s_fullyQualifiedMinimalFormat);

        var hintNameBuilder = new StringBuilder(fullyQualifiedName.Length);

        foreach (var character in fullyQualifiedName)
            hintNameBuilder.Append(char.IsLetterOrDigit(character) ? character : '_');

        return Invariant($"{hintNameBuilder}.g.cs");
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.EnumGenerators;
using System.Globalization;
using System.Text;
using static System.FormattableString;

namespace Neme.Extensions.SourceGenerators;

#pragma warning disable RS1038 // The package is installed
[Generator(LanguageNames.CSharp)]
#pragma warning restore RS1038
public sealed class EnumExtensionsGenerator : AbstractEnumGenerator, IIncrementalGenerator
{
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
                var configuredEnumTypes = value.Right!;

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

    private static void EmitEnumSource(
        SourceProductionContext context,
        INamedTypeSymbol enumType,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        Span<string?> typeSources =
            [
                EnumValueNamesGenerator.GetTypeMemberSource(context, 1, enumType, cancellationToken),
                EnumStaticNamesGenerator.GetTypeMemberSource(context, 1, enumType, cancellationToken)
            ];

        ReadOnlySpan<string?> flagExtensionSources = IsFlagsEnum(compilation, enumType)
            ? [
                EnumIncludesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken)!,
                EnumIncludesAllGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumIncludesAnyGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumStaticAllGenerator.GetExtensionSource(context, compilation, 2, enumType, cancellationToken),
                EnumStaticNoneGenerator.GetExtensionSource(context, compilation, 2, enumType, cancellationToken),
                EnumStaticFlagsDefinedGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
            ]
            : [];

        Span<string?> extensionSources =
            [
                EnumNameGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumToStringGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumValueNamesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumValueGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumStaticNamesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumStaticValuesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumStaticUnderlyingValuesGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
                EnumStaticIsDefinedGenerator.GetExtensionSource(context, 2, enumType, cancellationToken),
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
        var extensionClassName = GetExtensionClassName(enumType);

        cancellationToken.ThrowIfCancellationRequested();

        // lang=c#
        var innerSource = $$"""
                {{string.Join(Environment.NewLine, typeSources)}}

                    extension({{typeName}} @enum)
                    {
                {{string.Join(Environment.NewLine, extensionSources)}}
                    }
                """;

        var result = GetGeneratedClassSource(
            enumType.ContainingNamespace.IsGlobalNamespace
                ? null
                : enumType.ContainingNamespace?.ToDisplayString(),
            extensionClassName,
            innerSource,
            isPartial: false,
            emitAttribute: true,
            generatorName: nameof(EnumExtensionsGenerator),
            cancellationToken: cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        context.AddSource(GetFileName(enumType, "Extensions"), SourceText.From(result, Encoding.UTF8));
    }

    internal static string GetExtensionClassName(INamedTypeSymbol enumType)
    {
        var containingTypes = new Stack<string>();
        for (var containingType = enumType.ContainingType; containingType is not null; containingType = containingType.ContainingType)
            containingTypes.Push(containingType.Name);

        var typePrefix = containingTypes.Count == 0
            ? ""
            : string.Create(CultureInfo.InvariantCulture, $"""{string.Join("_", containingTypes)}_""");
        return string.Create(CultureInfo.InvariantCulture, $"""{typePrefix}{enumType.Name}GeneratedExtensions""");
    }
}

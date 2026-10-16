using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.EnumGenerators;
using System.Globalization;
using System.Text;

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
        ReadOnlySpan<IEnumGenerator> typeGenerators =
        [
            new EnumValueNamesGenerator(enumType),
            new EnumStaticNamesGenerator(enumType),
        ];

        ReadOnlySpan<IEnumGenerator> flagExtensionGenerators = IsFlagsEnum(compilation, enumType)
            ?
            [
                new EnumIncludesGenerator(enumType),
                new EnumIncludesAllGenerator(enumType),
                new EnumIncludesAnyGenerator(enumType),
                new EnumStaticAllGenerator(enumType, compilation),
                new EnumStaticNoneGenerator(enumType, compilation),
                new EnumStaticFlagsDefinedGenerator(enumType),
            ]
            : [];

        ReadOnlySpan<IEnumGenerator> extensionGenerators =
        [
            new EnumNameGenerator(enumType),
            new EnumToStringGenerator(enumType),
            new EnumValueNamesGenerator(enumType),
            new EnumValueGenerator(enumType),
            new EnumStaticNamesGenerator(enumType),
            new EnumStaticValuesGenerator(enumType),
            new EnumStaticUnderlyingValuesGenerator(enumType),
            new EnumStaticIsDefinedGenerator(enumType),
            ..flagExtensionGenerators,
        ];

        var typeSources = new string?[typeGenerators.Length];
        for (var i = 0; i < typeGenerators.Length; ++i)
            typeSources[i] = typeGenerators[i].GetTypeMemberSource(context, 1, cancellationToken);

        var extensionSources = new string?[extensionGenerators.Length];
        for (var i = 0; i < extensionGenerators.Length; ++i)
            extensionSources[i] = extensionGenerators[i].GetExtensionSource(context, 2, cancellationToken);

        EmitEnumExtension(
            context,
            typeSources.AsSpan().RemoveNullValuesInPlace(),
            extensionSources.AsSpan().RemoveNullValuesInPlace(),
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
            GetNamespaceName(enumType),
            extensionClassName,
            innerSource,
            isPartial: false,
            emitAttribute: true,
            generatorName: nameof(EnumExtensionsGenerator),
            cancellationToken: cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        context.AddSource(GetFileName(enumType, "Extensions"), SourceText.From(result, Encoding.UTF8));
    }

    private static string GetExtensionClassName(INamedTypeSymbol enumType)
    {
        var containingTypes = new Stack<string>();
        for (var containingType = enumType.ContainingType; containingType is not null; containingType = containingType.ContainingType)
            containingTypes.Push(containingType.Name);

        var typePrefix = containingTypes.Count == 0
            ? ""
            : string.Create(CultureInfo.InvariantCulture, $"""{string.Join("_", containingTypes)}_""");
        return string.Create(CultureInfo.InvariantCulture, $"""{typePrefix}{enumType.Name}GeneratedExtensions""");
    }

    private static string? GetNamespaceName(INamedTypeSymbol enumType)
    {
        return enumType.ContainingNamespace.IsGlobalNamespace
            ? null
            : enumType.ContainingNamespace.ToDisplayString();
    }

    internal static string GetFullyQualifiedExtensionsClassName(INamedTypeSymbol enumType)
    {
        var namespaceName = GetNamespaceName(enumType);
        var extensionClassName = GetExtensionClassName(enumType);
        var fullyQualifiedName = namespaceName is null
            ? extensionClassName
            : $"{namespaceName}.{extensionClassName}";
        return $"global::{fullyQualifiedName}";
    }
}

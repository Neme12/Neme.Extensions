using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.EnumGenerators;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        using var typeTextWriter = IndentedTextWriter.CreateNew(1);
        using var extensionTextWriter = IndentedTextWriter.CreateNew(2);

        ReadOnlySpan<IEnumGenerator> flagsGenerators = IsFlagsEnum(compilation, enumType)
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

        ReadOnlySpan<IEnumGenerator> generators =
            [
                new EnumNameGenerator(enumType),
                new EnumToStringGenerator(enumType),
                new EnumValueNamesGenerator(enumType, cancellationToken),
                new EnumValueGenerator(enumType),
                new EnumStaticNamesGenerator(enumType),
                new EnumStaticValuesGenerator(enumType),
                new EnumStaticUnderlyingValuesGenerator(enumType),
                new EnumStaticIsDefinedGenerator(enumType),
                ..flagsGenerators,
            ];

        foreach (var generator in generators)
        {
            if (generator.GetTypeMemberSource(context, typeTextWriter, cancellationToken))
                typeTextWriter.WriteLine();
        }

        cancellationToken.ThrowIfCancellationRequested();

        foreach (var generator in generators)
        {
            if (generator.GetExtensionSource(context, extensionTextWriter, cancellationToken))
                extensionTextWriter.WriteLine();
        }

        cancellationToken.ThrowIfCancellationRequested();

        EmitEnumExtension(
            context,
            typeTextWriter.InnerWriter.ToString(),
            extensionTextWriter.InnerWriter.ToString(),
            enumType,
            cancellationToken);
    }

    private static bool IsFlagsEnum(Compilation compilation, INamedTypeSymbol enumType, CancellationToken cancellationToken)
    {
        var flagsAttributeType = compilation.GetTypeByMetadataName("System.FlagsAttribute");
        if (flagsAttributeType is null)
            return false;

        foreach (var attribute in enumType.GetAttributes())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, flagsAttributeType))
                return true;
        }

        return false;
    }

    private static void EmitEnumExtension(
        SourceProductionContext context,
        string typeSource,
        string extensionSource,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var typeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var extensionClassName = GetExtensionClassName(enumType);

        cancellationToken.ThrowIfCancellationRequested();

        // lang=c#
        var innerSource = Invariant($$"""
                {{typeSource}}

                    extension({{typeName}} @enum)
                    {
                {{extensionSource}}
                    }
                """);

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
            : Invariant($"""{string.Join("_", containingTypes)}_""");
        return Invariant($"""{typePrefix}{enumType.Name}GeneratedExtensions""");
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
            : Invariant($"""{namespaceName}.{extensionClassName}""");
        return Invariant($"""global::{fullyQualifiedName}""");
    }
}

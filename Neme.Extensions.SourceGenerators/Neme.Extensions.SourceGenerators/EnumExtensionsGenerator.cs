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

                var tokenStart = 0;

                while (TryGetNextSemicolonSeparatedToken(configuredEnumTypes.AsSpan(), ref tokenStart, out var metadataName))
                {
                    sourceContext.CancellationToken.ThrowIfCancellationRequested();

                    var normalizedMetadataName = metadataName.Trim();
                    if (normalizedMetadataName.Length == 0)
                        continue;

                    if (compilation.GetTypeByMetadataName(normalizedMetadataName.ToString()) is not INamedTypeSymbol enumType)
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

    private static bool TryGetNextSemicolonSeparatedToken(
        ReadOnlySpan<char> input,
        ref int tokenStart,
        out ReadOnlySpan<char> token)
    {
        if (tokenStart > input.Length)
        {
            token = default;
            return false;
        }

        var remaining = input[tokenStart..];
        var separatorIndex = remaining.IndexOf(';');

        if (separatorIndex < 0)
        {
            token = remaining;
            tokenStart = input.Length + 1;
            return true;
        }

        token = remaining[..separatorIndex];
        tokenStart += separatorIndex + 1;
        return true;
    }

    private static void EmitEnumSource(
        SourceProductionContext context,
        INamedTypeSymbol enumType,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        using var typeTextWriter = IndentedTextWriter.CreateNew(1);
        using var extensionTextWriter = IndentedTextWriter.CreateNew(2);

        ReadOnlySpan<IEnumGenerator> flagsGenerators = IsFlagsEnum(compilation, enumType, cancellationToken)
            ?
            [
                new EnumIncludesGenerator(enumType, enumTypeName),
                new EnumIncludesAllGenerator(enumType, enumTypeName),
                new EnumIncludesAnyGenerator(enumType, enumTypeName),
                new EnumStaticAllGenerator(enumType, enumTypeName, compilation),
                new EnumStaticNoneGenerator(enumType, enumTypeName, compilation),
                new EnumStaticFlagsDefinedGenerator(enumType, enumTypeName),
            ]
            : [];

        ReadOnlySpan<IEnumGenerator> generators =
            [
                new EnumNameGenerator(enumType, enumTypeName),
                new EnumToStringGenerator(enumType, enumTypeName),
                new EnumValueNamesGenerator(enumType, enumTypeName, cancellationToken),
                new EnumValueGenerator(enumType),
                new EnumStaticNamesGenerator(enumType, enumTypeName),
                new EnumStaticValuesGenerator(enumType, enumTypeName),
                new EnumStaticUnderlyingValuesGenerator(enumType, enumTypeName),
                new EnumStaticIsDefinedGenerator(enumType, enumTypeName),
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
        var stringBuilder = new StringBuilder(32);

        AppendContainingTypeNames(stringBuilder, enumType.ContainingType);

        stringBuilder.Append(enumType.Name);
        stringBuilder.Append("GeneratedExtensions");

        return stringBuilder.ToString();

        static void AppendContainingTypeNames(StringBuilder stringBuilder, INamedTypeSymbol? containingType)
        {
            if (containingType is null)
                return;

            AppendContainingTypeNames(stringBuilder, containingType.ContainingType);
            stringBuilder.Append(containingType.Name);
            stringBuilder.Append('_');
        }
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

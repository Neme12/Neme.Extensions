using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.Internal;
using Neme.Extensions.SourceGenerators.RequireGenerators;
using System.CodeDom.Compiler;
using System.Text;

namespace Neme.Extensions.SourceGenerators;

#pragma warning disable RS1038 // The package is installed
[Generator(LanguageNames.CSharp)]
#pragma warning restore RS1038
public sealed class EnumRequireGenerator : AbstractEnumGenerator, IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumTypesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is EnumDeclarationSyntax,
                static (syntaxContext, cancellationToken) => GetEnumSymbol(syntaxContext, cancellationToken))
            .Where(static symbol => symbol is INamedTypeSymbol enumSymbol && IsAccessibleFromNamespaceLevel(enumSymbol));

        context.RegisterSourceOutput(
            context.CompilationProvider,
            static (context, compilation) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var requireGeneratedClassSource = GetGeneratedClassSource(
                GetNamespace(),
                "RequireGenerated",
                "",
                isPartial: true,
                emitAttribute: true,
                generatorName: nameof(EnumRequireGenerator),
                cancellationToken: context.CancellationToken);

            context.AddSource("RequireGenerated.g.cs", SourceText.From(requireGeneratedClassSource, Encoding.UTF8));
        });

        context.RegisterSourceOutput(
            enumTypesProvider,
            (context, enumType) =>
            {
                if (enumType is null)
                    return;

                EmitRequireSource(context, enumType, context.CancellationToken);
            });
    }

    internal static void EmitRequireSource(
        SourceProductionContext context,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        using var textWriter = IndentedTextWriter.CreateNew(1, 2048);

        var generationContext = new RequireGenerationContext
        {
            EnumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            ExtensionClassName = EnumExtensionsGenerator.GetFullyQualifiedExtensionsClassName(enumType),
        };

        ReadOnlySpan<IRequireGenerator> generators =
        [
            new RequireArgumentDefinedGenerator(generationContext),
            new RequireValueDefinedGenerator(generationContext),
        ];

        foreach (var generator in generators)
        {
            if (generator.GetExtensionSource(context, textWriter, cancellationToken))
                textWriter.WriteLine();
        }

        EmitRequireClass(
            context,
            textWriter.InnerWriter.ToString(),
            enumType,
            cancellationToken);
    }

    internal static void EmitRequireClass(
        SourceProductionContext context,
        string memberSource,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var extensionClassName = "RequireGenerated";

        cancellationToken.ThrowIfCancellationRequested();

        // lang=c#
        var innerSource = Invariant($"""
                {memberSource}
                """);

        cancellationToken.ThrowIfCancellationRequested();

        var result = GetGeneratedClassSource(
            GetNamespace(),
            extensionClassName,
            innerSource,
            isPartial: true,
            emitAttribute: false,
            generatorName: nameof(EnumRequireGenerator),
            cancellationToken: cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        context.AddSource(GetFileName(enumType, "Require"), SourceText.From(result, Encoding.UTF8));
    }

    private static string GetNamespace()
    {
        return "Neme.Generated.Contracts";
    }
}

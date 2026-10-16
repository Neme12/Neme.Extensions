using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.RequireGenerators;
using System.Globalization;
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

        var compilationProvider = context.CompilationProvider;

        context.RegisterSourceOutput(compilationProvider, static (context, compilation) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var @namespace = GetNamespace(compilation.Assembly);

            var requireGeneratedClassSource = GetGeneratedClassSource(
                @namespace,
                "RequireGenerated",
                "",
                isPartial: true,
                emitAttribute: true,
                generatorName: nameof(EnumRequireGenerator),
                cancellationToken: context.CancellationToken);

            context.AddSource("RequireGenerated.g.cs", SourceText.From(requireGeneratedClassSource, Encoding.UTF8));
        });

        context.RegisterSourceOutput(enumTypesProvider!, static (context, enumType) =>
        {
            EmitRequireSource(context, enumType!, context.CancellationToken);
        });
    }

    internal static void EmitRequireSource(
        SourceProductionContext context,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        Span<string?> typeSources =
            [
                RequireArgumentDefinedGenerator.GetSource(context, 1, enumType, cancellationToken),
            ];

        EmitRequireClass(
            context,
            typeSources.RemoveNullValuesInPlace(),
            enumType,
            cancellationToken);
    }

    internal static void EmitRequireClass(
        SourceProductionContext context,
        ReadOnlySpan<string> memberSources,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var typeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var extensionClassName = "RequireGenerated";

        cancellationToken.ThrowIfCancellationRequested();

        // lang=c#
        var innerSource = $$"""
                {{string.Join(Environment.NewLine, memberSources)}}
                """;

        var @namespace = GetNamespace(enumType.ContainingAssembly);

        cancellationToken.ThrowIfCancellationRequested();

        var result = GetGeneratedClassSource(
            @namespace,
            extensionClassName,
            innerSource,
            isPartial: true,
            emitAttribute: false,
            generatorName: nameof(EnumRequireGenerator),
            cancellationToken: cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        context.AddSource(GetFileName(enumType, "Require"), SourceText.From(result, Encoding.UTF8));
    }

    private static string GetNamespace(IAssemblySymbol assembly)
    {
        var projectNamespace = assembly.ContainingNamespace;

        var namespaceSuffix = "Generated.Contracts";
        return projectNamespace is null ||  projectNamespace.IsGlobalNamespace
            ? namespaceSuffix
            : string.Create(
                CultureInfo.InvariantCulture,
                $"{projectNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{namespaceSuffix}");
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Neme.Extensions.SourceGenerators.Internal;
using Neme.Extensions.SourceGenerators.RequireGenerators;
using System.CodeDom.Compiler;
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

        context.RegisterSourceOutput(
            context.CompilationProvider,
            static (context, compilation) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var requireGeneratedClassSource = GetGeneratedClassSource(
                GetNamespace(),
                "RequireGenerated",
                capacity: 256,
                new NullSourceEmitter(),
                isPartial: true,
                emitAttribute: true,
                generatorName: nameof(EnumRequireGenerator),
                cancellationToken: context.CancellationToken);

            context.AddSource(
                "RequireGenerated.g.cs",
                new StringBuilderText(requireGeneratedClassSource, Encoding.UTF8, SourceHashAlgorithm.Sha256));
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
        var generationContext = new RequireGenerationContext
        {
            EnumTypeName = enumType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            ExtensionClassName = EnumExtensionsGenerator.GetFullyQualifiedExtensionsClassName(enumType),
        };

        ImmutableArray<IRequireGenerator> generators =
        [
            new RequireArgumentDefinedGenerator(generationContext),
            new RequireValueDefinedGenerator(generationContext),
        ];

        EmitRequireClass(
            context,
            new MemberSourceEmitter(context, generators),
            enumType,
            cancellationToken);
    }

    internal static void EmitRequireClass<TEmitSource>(
        SourceProductionContext context,
        TEmitSource memberSourceEmitter,
        INamedTypeSymbol enumType,
        CancellationToken cancellationToken)
        where TEmitSource : struct, ISourceEmitter
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = GetGeneratedClassSource(
            GetNamespace(),
            "RequireGenerated",
            capacity: 2048,
            memberSourceEmitter,
            isPartial: true,
            emitAttribute: false,
            generatorName: nameof(EnumRequireGenerator),
            cancellationToken: cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        context.AddSource(
            GetFileName(enumType, "Require"),
            new StringBuilderText(result, Encoding.UTF8, SourceHashAlgorithm.Sha256));
    }

    private static string GetNamespace()
    {
        return "Neme.Generated.Contracts";
    }

    private readonly struct NullSourceEmitter : ISourceEmitter
    {
        public void EmitSource(StringBuilder builder, CancellationToken cancellationToken)
        {
        }
    }

    private readonly struct MemberSourceEmitter : ISourceEmitter
    {
        private readonly SourceProductionContext _context;
        private readonly ImmutableArray<IRequireGenerator> _generators;

        public MemberSourceEmitter(SourceProductionContext context, ImmutableArray<IRequireGenerator> generators)
        {
            _context = context;
            _generators = generators;
        }

        public void EmitSource(StringBuilder builder, CancellationToken cancellationToken)
        {
            using var textWriter = new IndentedTextWriter(new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                Indent = 1,
            };

            for (int i = 0; i < _generators.Length; ++i)
            {
                var generator = _generators[i];
                cancellationToken.ThrowIfCancellationRequested();

                if (generator.EmitExtensionSource(_context, textWriter, cancellationToken) && i < _generators.Length - 1)
                    textWriter.WriteLine();
            }

            textWriter.Flush();
        }
    }
}

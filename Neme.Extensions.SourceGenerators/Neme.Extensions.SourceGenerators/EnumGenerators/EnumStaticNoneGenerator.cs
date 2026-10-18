using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumStaticNoneGenerator(INamedTypeSymbol enumType, string enumTypeName, Compilation compilation) : IEnumGenerator
{
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly string _enumTypeName = enumTypeName;
    private readonly Compilation _compilation = compilation;

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        return false;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (GeneratorUtilities.HasConflictingFieldOrProperty(_compilation, _enumType, "None", cancellationToken))
            return false;

        textWriter.WriteLine($"public static {_enumTypeName} None =>");

        using (textWriter.AddIndent())
            textWriter.WriteLine("default;");

        return true;
    }
}

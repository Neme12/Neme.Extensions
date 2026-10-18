namespace Neme.Extensions.SourceGenerators.RequireGenerators;

internal readonly record struct RequireGenerationContext
{
    public required string EnumTypeName { get; init; }

    public required string ExtensionClassName { get; init; }
}

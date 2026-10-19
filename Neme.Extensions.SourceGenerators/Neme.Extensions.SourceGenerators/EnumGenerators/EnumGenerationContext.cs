using Microsoft.CodeAnalysis;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal readonly record struct EnumGenerationContext
{
    public required Compilation Compilation { get; init; }

    public required INamedTypeSymbol EnumType { get; init; }

    public required string EnumTypeName { get; init; }

    public required bool IsFlagsEnum { get; init; }

    public required ImmutableArray<IFieldSymbol> EnumMembers { get; init; }

    public required ImmutableArray<IFieldSymbol> ConstantEnumMembers { get; init; }

    public required string? UnderlyingTypeName { get; init; }
}

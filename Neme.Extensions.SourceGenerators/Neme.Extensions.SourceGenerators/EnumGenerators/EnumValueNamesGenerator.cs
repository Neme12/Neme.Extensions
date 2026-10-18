using Microsoft.CodeAnalysis;
using Neme.Extensions.SourceGenerators.Internal;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Neme.Extensions.SourceGenerators.EnumGenerators;

internal sealed class EnumValueNamesGenerator : IEnumGenerator
{
    private readonly string _enumTypeName;
    private readonly NameData _nameData;

    public EnumValueNamesGenerator(EnumGenerationContext generationContext, CancellationToken cancellationToken)
    {
        _enumTypeName = generationContext.EnumTypeName;
        _nameData = GetNameData(generationContext.ConstantEnumMembers, cancellationToken);
    }

    public bool GetTypeMemberSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine("private static string[] ValueOrderedNames");
        textWriter.WriteLine("{");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("get => global::System.Threading.LazyInitializer.EnsureInitialized(ref field, static () =>");
            textWriter.WriteLine("[");

            using (textWriter.AddIndent())
            {
                foreach (var name in _nameData.AllNames)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"nameof({_enumTypeName}.{name}),");
                }
            }

            textWriter.WriteLine("]!);");
        }

        textWriter.WriteLine("}");
        return true;
    }

    public bool GetExtensionSource(
        SourceProductionContext context,
        IndentedTextWriter textWriter,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        textWriter.WriteLine("public global::System.ReadOnlySpan<string> ValueNames =>");

        using (textWriter.AddIndent())
        {
            textWriter.WriteLine("@enum switch");
            textWriter.WriteLine("{");

            using (textWriter.AddIndent())
            {
                foreach (var group in _nameData.Groups)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    textWriter.WriteLine($"{_enumTypeName}.{group.MemberName} => ValueOrderedNames.AsSpan({group.Start}, {group.Length}),");
                }

                textWriter.WriteLine("_ => [],");
            }

            textWriter.WriteLine("};");
        }

        cancellationToken.ThrowIfCancellationRequested();
        return true;
    }

    private static NameData GetNameData(ImmutableArray<IFieldSymbol> enumMembers, CancellationToken cancellationToken)
    {

        if (enumMembers.Length == 0)
            return new NameData([], []);

        var orderedMembers = new OrderedMember[enumMembers.Length];
        for (var i = 0; i < enumMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var member = enumMembers[i];
            orderedMembers[i] = new OrderedMember(member.Name, member.ConstantValue!, i);
        }

        Array.Sort(orderedMembers, static (left, right) =>
        {
            var valueComparison = ((IComparable)left.ConstantValue).CompareTo(right.ConstantValue);
            if (valueComparison != 0)
                return valueComparison;

            return left.DeclarationIndex.CompareTo(right.DeclarationIndex);
        });

        var allNames = new string[orderedMembers.Length];
        for (var i = 0; i < orderedMembers.Length; ++i)
            allNames[i] = orderedMembers[i].MemberName;

        var groupCount = 1;
        for (var i = 1; i < orderedMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (((IComparable)orderedMembers[i - 1].ConstantValue).CompareTo(orderedMembers[i].ConstantValue) != 0)
                ++groupCount;
        }

        var groups = new NameRange[groupCount];
        var groupIndex = 0;
        var groupStart = 0;

        for (var i = 1; i <= orderedMembers.Length; ++i)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var isGroupBoundary = i == orderedMembers.Length ||
                ((IComparable)orderedMembers[i - 1].ConstantValue).CompareTo(orderedMembers[i].ConstantValue) != 0;

            if (!isGroupBoundary)
                continue;

            groups[groupIndex++] = new NameRange(
                orderedMembers[groupStart].MemberName,
                groupStart,
                i - groupStart);
            groupStart = i;
        }

        return new NameData(allNames, groups);
    }

    private readonly record struct NameData(string[] AllNames, NameRange[] Groups);

    private readonly record struct NameRange(string MemberName, int Start, int Length);

    private readonly record struct OrderedMember(string MemberName, object ConstantValue, int DeclarationIndex);
}

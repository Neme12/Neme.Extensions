using System.Reflection;
using System.Runtime.InteropServices;

namespace Neme.Extensions.Internal;

internal sealed class EnumCache<TEnum>
    where TEnum : struct, Enum
{
    private ValueLazy<bool> _isFlagsLazy;
    private ValueLazy<ImmutableArray<TEnum>> _valuesLazy;
    private ValueLazy<ImmutableArray<ulong>> _uint64ValuesLazy;

    private bool IsFlags
    {
        get => _isFlagsLazy.EnsureInitialized(() =>
            typeof(TEnum).GetCustomAttribute<FlagsAttribute>() is not null);
    }

    private ImmutableArray<TEnum> Values
    {
        get
        {
            return _valuesLazy.EnsureInitialized(() => ImmutableCollectionsMarshal.AsImmutableArray(
#if NET5_0_OR_GREATER
                Enum.GetValues<TEnum>()
#else
                (TEnum[])Enum.GetValues(typeof(TEnum))
#endif
                ));
        }
    }

    private ImmutableArray<ulong> UInt64Values
    {
        get
        {
            return _uint64ValuesLazy.EnsureInitialized(() =>
            {
                var builder = ImmutableArray.CreateBuilder<ulong>(Values.Length);

                foreach (var value in Values)
                    builder.Add(Convert.ToUInt64(value));

                return builder.MoveToImmutable();
            });
        }
    }

    public static EnumCache<TEnum> Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, () => new EnumCache<TEnum>())!;
    }

    public bool FlagsDefined(TEnum value)
    {
        if (!IsFlags)
            ThrowNotFlags(value);

        ulong allFlags = default;

        foreach (var flag in UInt64Values)
            allFlags |= flag;

        return (Convert.ToUInt64(value) | allFlags) == allFlags;

        static void ThrowNotFlags(TEnum value) =>
            throw new ArgumentException2(nameof(value), value, "The enum type must be a [Flags] enum.");
    }
}

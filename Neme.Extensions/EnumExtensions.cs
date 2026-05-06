using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neme.Extensions;

public static class EnumExtensions
{
    extension<T>(T enumValue) where T : struct, Enum
    {
        [OverloadResolutionPriority(1)]
        public bool HasFlag(T flag)
        {
            return enumValue.HasFlag(flag);
        }
    }

    extension(Enum)
    {
        public static bool IsDefinedFlags<T>(T value)
            where T : struct, Enum
        {
            var flagsAttribute = typeof(T).GetCustomAttribute<FlagsAttribute>(); 
            if  (flagsAttribute is null)
                throw new InvalidOperationException("The enum type must be a [Flags] enum.");


            var values =
#if NET50_OR_GREATER
                Enum.GetValues<T>();
#else
                Enum.GetValues(typeof(T)).Cast<T>();
#endif

            ulong allFlags = default;

            foreach (var flag in values)
                allFlags |= Convert.ToUInt64(flag);

            return (Convert.ToUInt64(value) | allFlags) == allFlags;
        }
    }
}

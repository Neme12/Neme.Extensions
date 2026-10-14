using Neme.Extensions.Internal;

namespace Neme.Extensions;

public static class EnumExtensions
{
    extension<T>(T enumValue) where T : struct, Enum
    {
        public bool HasFlag(T flag)
        {
            return enumValue.HasFlag(flag);
        }
    }

    extension(Enum)
    {
        public static bool FlagsDefined<T>(T value)
            where T : struct, Enum
        {
            return EnumCache<T>.Instance.FlagsDefined(value);
        }
    }
}

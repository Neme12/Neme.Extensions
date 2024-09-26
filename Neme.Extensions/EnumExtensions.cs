namespace Neme.Extensions;

public static class EnumExtensions
{
    public static bool HasFlag<T>(this T value, T flag)
        where T : struct, Enum
    {
        return value.HasFlag(flag);
    }
}

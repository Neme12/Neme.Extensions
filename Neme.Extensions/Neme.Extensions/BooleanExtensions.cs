namespace Neme.Extensions;

public static class BooleanExtensions
{
    extension(bool)
    {
        public static bool operator<(bool left, bool right) =>
            !left && right;

        public static bool operator<=(bool left, bool right) =>
            !left || right;

        public static bool operator >(bool left, bool right) =>
            left && !right;

        public static bool operator >=(bool left, bool right) =>
            left || !right;
    }
}

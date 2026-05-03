namespace Neme.Extensions;

public static class GuidExtensions
{
    public const int ByteLength = 16;

    extension(Guid)
    {
        public static int ByteLength =>
            ByteLength;
    }
}

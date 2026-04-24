namespace Neme.Extensions.Ownership;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public sealed class BorrowedAttribute : Attribute
{
    public BorrowedAttribute()
    {
    }
}

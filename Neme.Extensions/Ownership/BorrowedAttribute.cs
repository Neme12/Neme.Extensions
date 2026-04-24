namespace Neme.Extensions.Ownership;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class BorrowedAttribute : Attribute
{
    public BorrowedAttribute()
    {
    }
}

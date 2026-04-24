namespace Neme.Extensions.Ownership;


[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public sealed class OwnershipTransferAttribute : Attribute
{
    public OwnershipTransferAttribute()
    {
    }
}

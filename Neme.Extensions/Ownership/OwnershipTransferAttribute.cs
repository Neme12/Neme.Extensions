namespace Neme.Extensions.Ownership;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public sealed class OwnershipTransferAttribute : Attribute
{
    public OwnershipTransferAttribute()
    {
    }

    /// <summary>
    /// Name of the parameter that if <c>true</c>, the ownership transfer does not occur.
    /// This is useful for <c>leaveOpen</c> parameters.
    /// </summary>
    public string? Unless { get; init; }
}

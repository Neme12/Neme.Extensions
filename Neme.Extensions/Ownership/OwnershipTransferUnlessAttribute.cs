namespace Neme.Extensions.Ownership;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public sealed class OwnershipTransferUnlessAttribute : Attribute
{
    /// <param name="parameterName">
    /// Name of the parameter that if <c>true</c>, the ownership transfer does not occur.
    /// This is useful for <c>leaveOpen</c> parameters.
    /// </param>
    public OwnershipTransferUnlessAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }

    /// <summary>
    /// Name of the parameter that if <c>true</c>, the ownership transfer does not occur.
    /// This is useful for <c>leaveOpen</c> parameters.
    /// </summary>
    public string ParameterName { get; }
}

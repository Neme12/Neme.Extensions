namespace Neme.Extensions.Ownership;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
public sealed class OwnershipTransferIfAttribute : Attribute
{
    /// <param name="parameterName">
    /// Name of the parameter that if <c>false</c>, the ownership transfer does not occur.
    /// This is useful for <c>ownsValue</c> parameters.
    /// </param>
    public OwnershipTransferIfAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }

    /// <summary>
    /// Name of the parameter that if <c>false</c>, the ownership transfer does not occur.
    /// This is useful for <c>ownsValue</c> parameters.
    /// </summary>
    public string ParameterName { get; }
}

namespace Neme.Extensions.Ownership;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class OwnedAttribute : Attribute
{
    public OwnedAttribute()
    {
    }
}

using Neme.Extensions.Win32.InteropServices;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.Win32.CompilerServices;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
public abstract class HResultConstantAttributeBase : CustomConstantAttribute
{
    private protected readonly HResult _value;

    private protected HResultConstantAttributeBase(HResult value)
    {
        _value = value;
    }

    public sealed override object? Value => _value;
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
public sealed class HResultConstantAttribute : HResultConstantAttributeBase
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public HResultConstantAttribute(uint value)
#pragma warning restore CA1019 // Define accessors for attribute arguments
        : base(new(value))
    {
	}

    public new HResult Value => _value;
}

using System.Reflection;

namespace Neme.Extensions.Reflection;

public static class InterfaceMappingExtensions
{
    public static MethodInfo GetImplementationMethod(this InterfaceMapping interfaceMapping, MethodInfo interfaceMethod)
    {
        if (!IsValid(interfaceMapping))
            throw new ArgumentException(null, nameof(interfaceMapping));

        if (interfaceMethod is null)
            throw new ArgumentNullException(nameof(interfaceMethod));

        var index = Array.IndexOf(interfaceMapping.InterfaceMethods, interfaceMethod);
        if (index == -1)
            throw new ArgumentException("The specified method is not part of the interface mapping.", nameof(interfaceMethod));

        return interfaceMapping.TargetMethods[index];

        static bool IsValid(InterfaceMapping interfaceMapping) =>
            interfaceMapping.InterfaceType is { } interfaceType &&
            interfaceMapping.TargetType is { } targetType &&
            interfaceMapping.InterfaceMethods is { } interfaceMethods &&
            interfaceMapping.TargetMethods is { } targetMethods &&
            interfaceMethods.Length == targetMethods.Length &&
            interfaceMethods.All(m => m.DeclaringType == interfaceType) &&
            targetMethods.All(m => m.DeclaringType!.IsAssignableFrom(targetType));
    }
}

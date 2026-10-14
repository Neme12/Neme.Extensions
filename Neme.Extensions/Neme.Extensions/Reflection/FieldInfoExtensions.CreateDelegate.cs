using Neme.Extensions.Contracts;
using System.Reflection;
using System.Reflection.Emit;

namespace Neme.Extensions.Reflection;

#if NETCOREAPP
public static partial class FieldInfoExtensions
{
    extension(FieldInfo field)
    {
        public Delegate CreateDelegate(Type delegateType)
        {
            Require.ArgumentNotNull(field);
            Require.ArgumentNotNull(delegateType);

            if (field.IsStatic)
                return CreateStaticFieldDelegate(field, delegateType);
            else
                return CreateOpenInstanceFieldDelegate(field, delegateType);
        }

        public Delegate CreateDelegate(Type delegateType, object? target)
        {
            Require.ArgumentNotNull(field);
            Require.ArgumentNotNull(delegateType);

            if (field.IsStatic)
                return CreateStaticFieldDelegate(field, delegateType);
            else if (target is null)
                return CreateOpenInstanceFieldDelegate(field, delegateType);
            else
                return CreateClosedInstanceFieldDelegate(field, delegateType, target);
        }

        public T CreateDelegate<T>() where T : Delegate =>
            (T)field.CreateDelegate(typeof(T));

        public T CreateDelegate<T>(object? target) where T : Delegate =>
            (T)field.CreateDelegate(typeof(T), target);
    }

    private static Delegate CreateStaticFieldDelegate(FieldInfo field, Type delegateType)
    {
        var method = new DynamicMethod(
            $"Field_{field.Name}",
            field.FieldType.MakeByRefType(),
            Type.EmptyTypes,
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldsflda, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType);
    }

    private static Delegate CreateOpenInstanceFieldDelegate(FieldInfo field, Type delegateType)
    {
        var method = new DynamicMethod(
            $"Field_{field.Name}",
            field.FieldType.MakeByRefType(),
            [field.DeclaringType!],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType);
    }

    private static Delegate CreateClosedInstanceFieldDelegate(FieldInfo field, Type delegateType, object target)
    {
        var method = new DynamicMethod(
            $"Field_{field.Name}",
            field.FieldType.MakeByRefType(),
            [field.DeclaringType!],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldflda, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType, target);
    }
}
#endif

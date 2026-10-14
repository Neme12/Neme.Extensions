using Neme.Extensions.Contracts;
using System.Reflection;
using System.Reflection.Emit;

namespace Neme.Extensions.Reflection;

public static partial class FieldInfoExtensions
{
    extension(FieldInfo field)
    {
        public Delegate CreateGetDelegate(Type delegateType)
        {
            Require.ArgumentNotNull(field);
            Require.ArgumentNotNull(delegateType);

            if (field.IsStatic)
                return CreateStaticFieldGetter(field, delegateType);
            else
                return CreateOpenInstanceFieldGetter(field, delegateType);
        }

        public Delegate CreateGetDelegate(Type delegateType, object? target)
        {
            Require.ArgumentNotNull(field);
            Require.ArgumentNotNull(delegateType);

            if (field.IsStatic)
                return CreateStaticFieldGetter(field, delegateType);
            else if (target is null)
                return CreateOpenInstanceFieldGetter(field, delegateType);
            else
                return CreateClosedInstanceFieldGetter(field, delegateType, target);
        }

        public T CreateGetDelegate<T>() where T : Delegate =>
            (T)field.CreateGetDelegate(typeof(T));

        public T CreateGetDelegate<T>(object? target) where T : Delegate =>
            (T)field.CreateGetDelegate(typeof(T), target);

    }

    private static Delegate CreateStaticFieldGetter(FieldInfo field, Type delegateType)
    {
        var method = new DynamicMethod(
            $"Get_{field.Name}",
            field.FieldType,
            Type.EmptyTypes,
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldsfld, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType);
    }

    private static Delegate CreateOpenInstanceFieldGetter(FieldInfo field, Type delegateType)
    {
        var method = new DynamicMethod(
            $"Get_{field.Name}",
            field.FieldType,
            [field.DeclaringType!],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType);
    }

    private static Delegate CreateClosedInstanceFieldGetter(FieldInfo field, Type delegateType, object target)
    {
        var method = new DynamicMethod(
            $"Get_{field.Name}",
            field.FieldType,
            [field.DeclaringType!],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType, target);
    }
}

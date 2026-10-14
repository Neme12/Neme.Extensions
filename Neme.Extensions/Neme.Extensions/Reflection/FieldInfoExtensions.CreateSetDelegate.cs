using Neme.Extensions.Contracts;
using System.Reflection;
using System.Reflection.Emit;

namespace Neme.Extensions.Reflection;

public static partial class FieldInfoExtensions
{
    extension(FieldInfo field)
    {
        public Delegate CreateSetDelegate(Type delegateType)
        {
            Require.ArgumentNotNull(field);
            Require.ArgumentNotNull(delegateType);

            if (field.IsStatic)
                return CreateStaticFieldSetter(field, delegateType);
            else
                return CreateOpenInstanceFieldSetter(field, delegateType);
        }

        public Delegate CreateSetDelegate(Type delegateType, object? target)
        {
            Require.ArgumentNotNull(field);
            Require.ArgumentNotNull(delegateType);

            if (field.IsStatic)
                return CreateStaticFieldSetter(field, delegateType);
            else if (target is null)
                return CreateOpenInstanceFieldSetter(field, delegateType);
            else
                return CreateClosedInstanceFieldSetter(field, delegateType, target);
        }

        public T CreateSetDelegate<T>() where T : Delegate =>
            (T)field.CreateSetDelegate(typeof(T));

        public T CreateSetDelegate<T>(object? target) where T : Delegate =>
            (T)field.CreateSetDelegate(typeof(T), target);
    }

    private static Delegate CreateStaticFieldSetter(FieldInfo field, Type delegateType)
    {
        var method = new DynamicMethod(
            $"Set_{field.Name}",
            typeof(void),
            [field.FieldType],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Stsfld, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType);
    }

    private static Delegate CreateOpenInstanceFieldSetter(FieldInfo field, Type delegateType)
    {
        var method = new DynamicMethod(
            $"Set_{field.Name}",
            typeof(void),
            [field.DeclaringType!, field.FieldType],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType);
    }

    private static Delegate CreateClosedInstanceFieldSetter(FieldInfo field, Type delegateType, object target)
    {
        var method = new DynamicMethod(
            $"Set_{field.Name}",
            typeof(void),
            [field.DeclaringType!, field.FieldType],
            field.DeclaringType!.Module,
            skipVisibility: true);

        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Stfld, field);
        il.Emit(OpCodes.Ret);

        return method.CreateDelegate(delegateType, target);
    }
}

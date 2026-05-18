using Neme.Extensions.Contracts;
using System.Reflection;
using System.Reflection.Emit;

namespace Neme.Extensions.Reflection;

public static class FieldInfoExtensions
{
    // CreateGetDelegate and CreateSetDelegate - work on all frameworks including .NET Framework
    extension(FieldInfo field)
    {
        public T GetValue<T>(object? obj) =>
            (T)field.GetValue(obj)!;

        public T GetValueDirect<T>(TypedReference obj) =>
            (T)field.GetValueDirect(obj)!;

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

    // Helper methods for getters (non-ref return)
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

    // Helper methods for setters
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

#if NETCOREAPP
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
#endif
}

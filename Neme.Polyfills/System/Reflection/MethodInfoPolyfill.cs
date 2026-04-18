namespace System.Reflection;

public static class MethodInfoPolyfill
{
    public static T CreateDelegate<T>(this MethodInfo method)
        where T : Delegate
    {
#if NET5_0_OR_GREATER
        return method.CreateDelegate<T>();
#else
        return (T)method.CreateDelegate(typeof(T));
#endif
    }

    public static T CreateDelegate<T>(this MethodInfo method, object? target)
        where T : Delegate
    {
#if NET5_0_OR_GREATER
        return method.CreateDelegate<T>(target);
#else
        return (T)method.CreateDelegate(typeof(T), target);
#endif
    }
}

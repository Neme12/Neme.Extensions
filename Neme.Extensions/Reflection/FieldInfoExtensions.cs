using System.Reflection;

namespace Neme.Extensions.Reflection;

public static partial class FieldInfoExtensions
{
    // CreateGetDelegate and CreateSetDelegate - work on all frameworks including .NET Framework
    extension(FieldInfo field)
    {
        public TValue GetValue<TValue>(object? obj) =>
            (TValue)field.GetValue(obj)!;

        public TValue GetValueDirect<TValue>(TypedReference obj) =>
            (TValue)field.GetValueDirect(obj)!;

        public object? GetValueDirect<TObject>(ref TObject obj)
        {
            var typedReference = __makeref(obj);
            return field.GetValueDirect(typedReference)!;
        }

        public TValue GetValueDirect<TObject, TValue>(ref TObject obj)
        {
            var typedReference = __makeref(obj);
            return (TValue)field.GetValueDirect(typedReference)!;
        }
    }
}

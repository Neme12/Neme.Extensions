using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Neme.Extensions.AspNetCore.Mvc.ModelBinding;

public sealed class Base64UrlGuidModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType == typeof(Guid))
            return new Base64UrlGuidModelBinder();

        return null;
    }
}

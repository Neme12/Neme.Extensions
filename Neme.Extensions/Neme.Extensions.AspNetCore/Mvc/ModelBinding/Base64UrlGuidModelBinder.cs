using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Neme.Extensions.AspNetCore.Routing;
using Neme.Extensions.AspNetCore.WebUtilities;

namespace Neme.Extensions.AspNetCore.Mvc.ModelBinding;

public sealed class Base64UrlGuidModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        var value = valueProviderResult.FirstValue;
        if (value.IsNullOrEmpty())
            return Task.CompletedTask;

        // Only handle valid Base64Url GUID format (22 chars)
        if (!Base64UrlGuidRouteConstraint.Base64UrlGuidPattern().IsMatch(value))
            return Task.CompletedTask;

        try
        {
            var guid = WebEncoders.Base64UrlDecodeGuid(value);
            bindingContext.Result = ModelBindingResult.Success(guid);
        }
        catch (FormatException)
        {
            bindingContext.ModelState.AddModelError(
                bindingContext.ModelName,
                "Invalid ID format.");
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}

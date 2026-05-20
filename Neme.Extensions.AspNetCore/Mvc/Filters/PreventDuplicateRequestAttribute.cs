using Microsoft.AspNetCore.Mvc.Filters;

namespace Neme.Extensions.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PreventDuplicateRequestAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await PreventDuplicateRequestFilter.BeforeActionExecutionAsync(context);
        await next();
    }
}

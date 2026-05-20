using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Neme.Extensions.AspNetCore.Mvc.Filters;

public sealed class PreventDuplicateRequestFilter : IAsyncActionFilter
{
    internal static async Task BeforeActionExecutionAsync(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;

        if (!httpContext.Request.Form.ContainsKey("__RequestVerificationToken"))
            return;

        var session = httpContext.Session;

        await session.LoadAsync(cancellationToken);

        var currentToken = httpContext.Request.Form["__RequestVerificationToken"].ToString();
        var lastToken = session.GetString("__LastProcessedToken");

        if (lastToken == currentToken)
        {
            context.ModelState.AddModelError(string.Empty, Strings.PreventDuplicateRequestAttribute_RequestSentRepeatedly);
            return;
        }

        session.SetString("__LastProcessedToken", currentToken);

        await session.CommitAsync(cancellationToken);
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await BeforeActionExecutionAsync(context);
        await next();
    }
}

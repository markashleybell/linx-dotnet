using System;
using System.Threading.Tasks;
using Linx.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Linx.Support
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ctx = context.HttpContext;

            if (!ctx.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userService = ctx.RequestServices.GetRequiredService<IUserService>();

            var (valid, _) = await userService.ValidateApiKey(apiKey);

            if (!valid)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}

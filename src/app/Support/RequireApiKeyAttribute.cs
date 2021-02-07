using System;
using System.Threading.Tasks;
using Linx.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using static Linx.Functions.Functions;

namespace Linx.Support
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var (apiKeyPresent, apiKey) = context.HttpContext.TryGetApiKey();

            if (!apiKeyPresent)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

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

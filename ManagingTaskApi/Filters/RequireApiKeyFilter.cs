using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagementApi.Models;

namespace TaskManagementApi.Filters;

public class RequireApiKeyFilter : IAsyncActionFilter
{
    private readonly IConfiguration _configuration;

    public RequireApiKeyFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "Missing X-API-KEY header."
            });
            return;
        }

        var apiKeys = _configuration.GetSection("ApiKeys").Get<List<ApiKeyEntry>>() ?? [];

        var matchedKey = apiKeys.FirstOrDefault(k => k.Key == extractedApiKey);

        if (matchedKey is null)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "Invalid API key."
            });
            return;
        }

        if (matchedKey.ExpiresAt <= DateTime.UtcNow)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                message = "API key has expired."
            });
            return;
        }

        await next();
    }
}
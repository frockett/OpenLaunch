using OpenLaunch.Services;

namespace OpenLaunch.Filters;

public class ApiKeyFilter : IEndpointFilter
{
    private readonly ApiKeyService _apiKeyService;
    
    public ApiKeyFilter(ApiKeyService apiKeyService) => _apiKeyService = apiKeyService;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        if (!httpContext.Request.Headers.TryGetValue("x-api-key", out var key)
            || !await _apiKeyService.IsApiKeyValid(key))
        {
            return Results.Unauthorized();
        }
        
        return await next(context);
    }
    
}
using Microsoft.AspNetCore.Mvc;
using OpenLaunch.Features.Subscribe.Models;
using OpenLaunch.Filters;

namespace OpenLaunch.Features.Subscribe;

public static class SubscribeEndpoint
{
    public static void MapSubscribeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/subscribe", async ([FromBody] SubscribeRequest request, SubscribeHandler handler) =>
            await handler.HandleSubscribeRequest(request))
            .AddEndpointFilter<ApiKeyFilter>()
            .WithName("SubscribeUser")
            .WithTags("Subscribe");
    }
}
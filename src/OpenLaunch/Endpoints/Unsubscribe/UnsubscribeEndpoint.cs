using OpenLaunch.Features.Unsubscribe.Models;
using OpenLaunch.Features.Unsubscribe.Services;

namespace OpenLaunch.Features.Unsubscribe;

public static class UnsubscribeEndpoint
{
    public static void MapUnsubscribeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/unsubscribe", async ([AsParameters] UnsubscribeRequest request, UnsubscribeHandler handler) =>
            await handler.HandleUnsubscribeRequest(request))
            .WithName("UnsubscribeUser")
            .WithTags("Unsubscribe")
            .AllowAnonymous();
    }
}
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OpenLaunch.Endpoints.EmailBounces;

public static class BounceEndpoint
{
    public static void MapBounceEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/sns/bounce", async (HttpRequest request, SnsBounceHandler handler) =>
            {
                using var reader = new StreamReader(request.Body);
                var body = await reader.ReadToEndAsync();
                
                await handler.HandleBounceNotification(body);

                return Results.Ok();
            })
            .WithName("HandleBounce")
            .WithTags("Bounce")
            .AllowAnonymous();
    }
}
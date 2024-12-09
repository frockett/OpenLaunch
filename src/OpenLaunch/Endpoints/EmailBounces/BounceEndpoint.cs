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
                
                Console.WriteLine(body);
                
                await handler.HandleBounceNotification(body);

                return Results.Ok();
                // var snsMessage = JsonSerializer.Deserialize<SnsNotification>(body);
                //
                // if (snsMessage.Type == "SubscriptionConfirmation")
                // {
                //     // Handle subscription confirmation
                //     using var httpClient = new HttpClient();
                //     await httpClient.GetAsync(snsMessage.SubscribeURL);
                //     return Results.Ok("Subscription confirmed.");
                // }
                // else if (snsMessage.Type == "Notification")
                // {
                //     // Handle bounce notification
                //     var bounceNotification = JsonSerializer.Deserialize<BounceNotification>(snsMessage.Message);
                //     await handler.HandleBounceNotification(bounceNotification);
                //     return Results.Ok("Bounce processed.");
                // }
            })
            .WithName("HandleBounce")
            .WithTags("Bounce")
            .AllowAnonymous();
    }
}
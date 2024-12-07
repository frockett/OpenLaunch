namespace OpenLaunch.Features.Upload;

public static class UploadEndpoint
{
    public static void MapUploadEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/upload-image", async (IFormFile file, UploadHandler handler) => 
            await handler.UploadImageAsync(file))
            .WithName("UploadImage")
            .WithTags("Upload")
            .RequireAuthorization();
    }
}
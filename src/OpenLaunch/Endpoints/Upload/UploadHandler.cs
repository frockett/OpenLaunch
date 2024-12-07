using Serilog;

namespace OpenLaunch.Features.Upload;

public class UploadHandler
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadHandler(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<IResult> UploadImageAsync(IFormFile file)
    {
        Log.Information("Upload image request received.");
        try
        {
            Log.Information("File size: {size:F2}KB", file.Length / 1024.0);
            var imagesDirectory = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imagesDirectory))
            {
                Log.Information("Images directory does not exist. Creating directory at location: {path}", imagesDirectory);
                Directory.CreateDirectory(imagesDirectory);
            }
            
            var fileName = $"upload-{DateTime.Today:yyyy-MM-dd}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(imagesDirectory, fileName);
            
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            
            var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
            var url = $"{baseUrl}/images/{fileName}";

            Log.Information("Image uploaded successfully. Upload image URL: {url}", url);
            return Results.Json(new { url });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error uploading image");
            return Results.Problem(detail: ex.Message, title: "Image upload failed", statusCode: 500);
        }
    }
}
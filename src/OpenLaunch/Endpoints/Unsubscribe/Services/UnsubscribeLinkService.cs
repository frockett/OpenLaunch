namespace OpenLaunch.Features.Unsubscribe.Services;

public class UnsubscribeLinkService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UnsubscribeTokenService _tokenService;

    public UnsubscribeLinkService(
        IHttpContextAccessor httpContextAccessor,
        UnsubscribeTokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }
    
    public async Task<string> GenerateUnsubscribeLink(string email)
    {
        var token = _tokenService.GenerateToken(email);
        var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
        return $"{baseUrl}/api/unsubscribe?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    }
}
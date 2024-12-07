using OpenLaunch.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLaunch.Features.Unsubscribe.Models;
using OpenLaunch.Features.Unsubscribe.Services;
using Serilog;

namespace OpenLaunch.Features.Unsubscribe;

public class UnsubscribeHandler
{
    private readonly AppDbContext _context;
    private readonly UnsubscribeTokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UnsubscribeHandler(
        AppDbContext context, 
        UnsubscribeTokenService tokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> HandleUnsubscribeRequest(UnsubscribeRequest request)
    {
        Log.Information("Unsubscribe request for email {email}", request.Email);
        
        bool isValid = await ValidateToken(request);
        Log.Information("Unsubscribe request for email {email} is valid: {isValid}", request.Email, isValid);

        if (!isValid)
        {
            Log.Warning("Unsubscribe request for email {email} had invalid token {token}, returned OK response anyway", request.Email, request.Token);
            return new OkObjectResult(new { message = "User successfully unsubscribed" });
        }

        try
        {
            var user = _context.WaitlistSignups.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                Log.Warning("Unsubscribe request for email {email} is invalid, user does not exist", request.Email);
                return new OkObjectResult(new { message = "User successfully unsubscribed" });
            }

            user.UpdateConsent = false;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error("An error occurred while attempting to unsubscriber email {email}", request.Email);
        }
        
        return new OkObjectResult(new { message = "User successfully unsubscribed" });
    }

    // public async Task<string> GenerateUnsubscribeLink(string email)
    // {
    //     var token = _tokenService.GenerateToken(email);
    //     var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
    //     return $"{baseUrl}/api/unsubscribe?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    // }

    private async Task<bool> ValidateToken(UnsubscribeRequest request)
    {
        var user = await _context.WaitlistSignups.FirstOrDefaultAsync(w => w.Email == request.Email);
        if (user == null) return false;
        
        var computedToken = _tokenService.GenerateToken(user.Email);
        return computedToken == request.Token;
    }
}
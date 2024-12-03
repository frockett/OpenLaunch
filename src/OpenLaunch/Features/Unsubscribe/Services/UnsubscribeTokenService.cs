using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OpenLaunch.Features.Unsubscribe.Services;

public class UnsubscribeTokenService
{
    private readonly string _unsubscribeKey;

    public UnsubscribeTokenService(IConfiguration config)
    {
        _unsubscribeKey = config["UNSUBSCRIBE_KEY"] ?? 
                          throw new ArgumentNullException("Unsubscribe_Key invalid or not found");
    }

    public string GenerateToken(string email)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_unsubscribeKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(email.ToLower()));
        return Base64UrlEncoder.Encode(hash);
    }
}
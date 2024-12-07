using System.Security.Cryptography;

namespace OpenLaunch.Utils;

public static class ApiKeyGenerator
{
    public static string GenerateApiKey(int length = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var keyBytes = new byte[length];
        rng.GetBytes(keyBytes);
        return Convert.ToBase64String(keyBytes).Replace("+", "").Replace("/", "").Replace("=", "");
    }
}
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OpenLaunch.Data;
using OpenLaunch.Models;
using OpenLaunch.Utils;
using Serilog;

namespace OpenLaunch.Services;

public class ApiKeyService
{
    private readonly AppDbContext _context;

    public ApiKeyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsApiKeyValid(string apiKey)
    {
        return await _context.ApiKeys.AnyAsync(k => k.Key == apiKey && k.IsActive);
    }

    public async Task<List<ApiKey>> GetAllKeysAsync()
    {
        return await _context.ApiKeys.ToListAsync();
    }

    public async Task<ApiKey?> CreateApiKeyAsync(string name = null, string description = null)
    {
        try
        {
            var apiKey = new ApiKey
            {
                Key = ApiKeyGenerator.GenerateApiKey(),
                Name = name,
                Description = description,
                IsActive = true
            };

            var result = await _context.ApiKeys.AddAsync(apiKey);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating API key");
            return null;
        }
    }

    public async Task<bool> DeleteApiKeyAsync(ApiKey apiKey)
    {
        try
        {
            _context.ApiKeys.Remove(apiKey);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting API key");
            return false;
        }
    }
    
    
}
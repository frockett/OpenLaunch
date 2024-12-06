using Microsoft.EntityFrameworkCore;
using OpenLaunch.Data;
using OpenLaunch.Models;
using Serilog;

namespace OpenLaunch.Services;

public class FromAddressService
{
    private readonly AppDbContext _dbContext;

    public FromAddressService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<FromAddress?>> FetchAllAsync()
    {
        return await _dbContext.FromAddresses.ToListAsync();
    }

    public async Task<FromAddress?> AddAsync(string address)
    {
        try
        {
            var newAddress = new FromAddress { Address = address };
            var result = await _dbContext.FromAddresses.AddAsync(newAddress);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to add From Address");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(FromAddress fromAddress)
    {
        try
        {
            _dbContext.Remove(fromAddress);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to delete From Address");
            return false;
        }
    }
}
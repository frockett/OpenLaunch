using Microsoft.EntityFrameworkCore;
using OpenLaunch.Data;
using OpenLaunch.Models;
using Serilog;

namespace OpenLaunch.Services;

public class BounceService
{
    private readonly AppDbContext _context;

    public BounceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bounces>?> GetAllAsync()
    {
        try
        {
            var bounces = await _context.Bounces
                .Include(x => x.BouncedSignup)
                .ToListAsync();
            return bounces;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting bounces");
            return null;
        }
    }

    public async Task<Bounces?> UnquarantineAsync(Bounces bounce)
    {
        try
        {
            var bounceToUnquarantine = 
                await _context.Bounces
                    .FirstOrDefaultAsync(x => x.Id == bounce.Id);

            if (bounceToUnquarantine == null)
                return null;

            var signupToUnquarantine =
                await _context.WaitlistSignups
                    .FirstOrDefaultAsync(x => x.Id == bounceToUnquarantine.BouncedSignup.Id);

            if (signupToUnquarantine == null)
                return null;
                    
            signupToUnquarantine.HasBounced = false;
            
            _context.Bounces.Remove(bounceToUnquarantine); // Remove the bounce entry from the DB for clarity
            await _context.SaveChangesAsync();
            
            return bounceToUnquarantine;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting bounce to unquarantine");
            return null;
        }
    }

    public async Task<bool> DeleteAssociatedSignupAsync(Bounces bounce)
    {
        try
        {
            var bounceToDelete = await _context.Bounces.FirstOrDefaultAsync(x => x.Id == bounce.Id);

            if (bounceToDelete == null)
                return false;

            var signupToDelete = await _context.WaitlistSignups.FindAsync(bounce.BouncedSignup.Id);

            if (signupToDelete == null)
                return false;

            _context.WaitlistSignups.Remove(signupToDelete);
            _context.Bounces.Remove(bounceToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting signup associated with bounce");
            return false;
        }
    }
}
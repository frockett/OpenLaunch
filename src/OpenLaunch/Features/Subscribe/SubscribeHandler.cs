using Microsoft.EntityFrameworkCore;
using OpenLaunch.Data;
using OpenLaunch.Features.Subscribe.Models;
using OpenLaunch.Models;
using Serilog;

namespace OpenLaunch.Features.Subscribe;

public class SubscribeHandler
{
    private readonly AppDbContext _context;

    public SubscribeHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleSubscribeRequest(SubscribeRequest subscribeRequest)
    {
        try
        {
            var signup = await _context.WaitlistSignups.FirstOrDefaultAsync(x => x.Email == subscribeRequest.Email);

            if (signup != null)
            {
                Log.Warning("Duplicate subscription request for email {email}", signup.Email);
                return Results.BadRequest("Duplicate email");
            }

            var newSignup = new WaitlistSignup
            {
                Email = subscribeRequest.Email,
                BetaConsent = subscribeRequest.BetaConsent,
                UpdateConsent = subscribeRequest.UpdateConsent,
            };

            var createResult = await _context.WaitlistSignups.AddAsync(newSignup);
            await _context.SaveChangesAsync();
            
            Log.Information("New signup added successfully for email {email}", createResult.Entity.Email);
            return Results.Ok("New subscription added successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occured while adding subscription.");
            return Results.Problem("An error occured while adding subscription.");
        }
    }
}
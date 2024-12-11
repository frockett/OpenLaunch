using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.EntityFrameworkCore;
using OpenLaunch.Data;
using OpenLaunch.Endpoints.EmailBounces.AwsSns.Models;
using OpenLaunch.Models;
using Serilog;

namespace OpenLaunch.Endpoints.EmailBounces;

public class SnsBounceHandler
{
    private readonly AppDbContext _context;

    public SnsBounceHandler(AppDbContext context)
    {
        _context = context;
    }
    public async Task HandleBounceNotification(string snsMessage)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var bounceNotification = JsonSerializer.Deserialize<BounceNotification>(snsMessage, options);

            if (bounceNotification?.Message != null)
            {
                var bounceMessage = JsonSerializer.Deserialize<BounceMessage>(bounceNotification.Message, options);

                if (bounceMessage?.NotificationType == "Bounce" && bounceMessage?.Bounce.BounceType == "Permanent")
                {
                    foreach (var recipient in bounceMessage.Bounce.BouncedRecipients)
                    {
                        Log.Information(
                            "Bounce notification received for email: {EmailAddress}, bounce type: {BounceType}, reason: {DiagnosticCode}",
                            recipient.EmailAddress, bounceMessage.Bounce.BounceType, recipient.DiagnosticCode);

                        await UnsubscribeBouncedEmail(recipient);
                    }
                }
                else
                {
                    Log.Information("Transient bounce notification received. Details: {details}", bounceMessage);
                }
            }
            else
            {
                Log.Warning("Invalid bounce notification received. Details: {details}", bounceNotification);
            }
        }
        catch (JsonException ex)
        {
            Log.Error(ex, "A Json deserialization error occurred.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unhandled exception occurred.");
        }
    }

    public async Task UnsubscribeBouncedEmail(BouncedRecipient recipient)
    {
        try
        {
            var bouncedUser = await _context.WaitlistSignups
                .FirstOrDefaultAsync(u => u.Email == recipient.EmailAddress);

            if (bouncedUser == null)
            {
                Log.Warning("Bounce notification received for nonexistent user {email}", recipient.EmailAddress);
                return;
            }

            bouncedUser.HasBounced = true;
            bouncedUser.UpdateConsent = false;

            var bounceRecord = new Bounces
            {
                BouncedSignup = bouncedUser,
                BouncedTime = DateTime.UtcNow,
                DiagnosticCode = recipient.DiagnosticCode,
            };

            await _context.Bounces.AddAsync(bounceRecord);
            await _context.SaveChangesAsync();
            Log.Information("Bounced user with email {email} has been successfully unsubscribed, flagged, and the bounce has been recorded.", recipient.EmailAddress);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while logging and unsubscribing bounced email {email}", recipient.EmailAddress);
        }
    }
}

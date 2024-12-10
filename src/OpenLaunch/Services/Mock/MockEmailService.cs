using OpenLaunch.Interfaces;
using OpenLaunch.Models;
using Serilog;

namespace OpenLaunch.Services;

public class MockEmailService : IEmailService
{
    public async Task<EmailSendResult> SendEmailAsync(
        string fromEmailAddress,
        List<string> toEmailAddresses,
        string? subject,
        string? htmlContent,
        string? textContent,
        string? displayName = null,
        string? templateName = null,
        string? templateData = null,
        string? contactListName = null)
    {
        var result = new EmailSendResult();
        var random = new Random();
        
        await Task.Delay(100);

        foreach (var toEmailAddress in toEmailAddresses)
        {
            if (random.Next(2) == 0)
            {
                result.SuccessfullySentEmails.Add(toEmailAddress);
            }
            else
            {
                result.FailedEmails.Add((toEmailAddress, "Simulated Failure: Test error message"));
            }
        }

        return result;
    }
}
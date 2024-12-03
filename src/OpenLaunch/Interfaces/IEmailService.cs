using OpenLaunch.Models;

namespace OpenLaunch.Interfaces;

public interface IEmailService
{
    public Task<EmailSendResult> SendEmailAsync(string fromEmailAddress,
        List<string> toEmailAddresses,
        string? subject,
        string? htmlContent,
        string? textContent,
        string? displayName = null,
        string? templateName = null,
        string? templateData = null,
        string? contactListName = null);
}
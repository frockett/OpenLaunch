namespace OpenLaunch.Models;

public class EmailSendResult
{
    public List<string> SuccessfullySentEmails { get; set; } = new();
    public List<(string Email, string ErrorMessage)> FailedEmails { get; set; } = new();
}
namespace OpenLaunch.Features.Subscribe.Models;

public class SubscribeRequest
{
    public required string Email { get; set; }
    public bool BetaConsent { get; set; }
    public bool UpdateConsent { get; set; }
}
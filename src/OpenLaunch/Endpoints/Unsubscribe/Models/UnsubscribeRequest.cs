using Microsoft.AspNetCore.Mvc;

namespace OpenLaunch.Features.Unsubscribe.Models;

public record UnsubscribeRequest
{
    [FromQuery] public required string Email { get; init; }
    [FromQuery] public required string Token { get; init; }
}
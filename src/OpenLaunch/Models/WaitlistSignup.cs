using System.ComponentModel.DataAnnotations;

namespace OpenLaunch.Models;

public class WaitlistSignup
{
    [Key]
    public int Id { get; set; }
    [EmailAddress]
    [Required]
    public required string Email { get; set; }

    public bool UpdateConsent { get; set; } = true;
    public bool BetaConsent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
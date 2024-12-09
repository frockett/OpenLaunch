using System.ComponentModel.DataAnnotations;

namespace OpenLaunch.Models;

public class Bounces
{
    [Key] public int Id { get; set; }
    public WaitlistSignup? BouncedSignup { get; set; }
    public string? DiagnosticCode { get; set; }
    public DateTime BouncedTime { get; set; }
}
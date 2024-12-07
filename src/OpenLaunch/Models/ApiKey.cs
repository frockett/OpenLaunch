using System.ComponentModel.DataAnnotations;

namespace OpenLaunch.Models;

public class ApiKey
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Key { get; set; }
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
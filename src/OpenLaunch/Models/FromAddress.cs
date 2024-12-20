using System.ComponentModel.DataAnnotations;

namespace OpenLaunch.Models;

public class FromAddress
{
    [Key]
    public int Id { get; set; }
    
    [EmailAddress] [Required] 
    public required string Address { get; set; }
    
    [MaxLength(128)]
    public string? DisplayName { get; set; }
    
}
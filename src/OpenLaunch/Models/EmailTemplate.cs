using System.ComponentModel.DataAnnotations;

namespace OpenLaunch.Models;

public class EmailTemplate
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    [Required] [DataType(DataType.Html)]
    public required string HtmlContent { get; set; }
}
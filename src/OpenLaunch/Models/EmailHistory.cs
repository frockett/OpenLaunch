using System.ComponentModel.DataAnnotations;

namespace OpenLaunch.Models;

public class EmailHistory
{
    [Key]
    public int Id { get; set; }
    
    public required string HtmlContent { get; set; }
    
    public required List<string> ToAddresses { get; set; } = new List<string>();
    
    public required string FromAddress { get; set; }
    
    public List<string> SuccessfulRecipients { get; set; } = new List<string>();
    public List<string> FailedRecipients { get; set; } = new List<string>();
}
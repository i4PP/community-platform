using System.ComponentModel.DataAnnotations;

namespace BMW.ASP.Models;

public class JoinClubViewModel
{
    [Required]
    public string? InviteCode { get; set; }
    
}
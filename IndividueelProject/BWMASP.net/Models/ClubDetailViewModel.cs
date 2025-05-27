namespace BMW.ASP.Models;

public class ClubDetailViewModel
{
    public int ClubId { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? Land { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<RolesDetailViewModel>? Roles { get; set; }
    
    public List<ClubMembershipViewModel>? Members { get; set; }
    
}
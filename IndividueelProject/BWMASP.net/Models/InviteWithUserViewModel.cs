namespace BMW.ASP.Models;

public class InviteWithUserViewModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int ClubId { get; set; }
    public int MaxUses { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string? UserName { get; set; }
    public int UserId { get; set; }
    
    
}
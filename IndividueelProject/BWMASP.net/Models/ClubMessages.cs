using BMWDomain.Entities;

namespace BMW.ASP.Models;

public class ClubMessages
{
    public int ClubId { get; set; }
    
    public List<MessageViewModel>? Messages { get; set; }
    
}
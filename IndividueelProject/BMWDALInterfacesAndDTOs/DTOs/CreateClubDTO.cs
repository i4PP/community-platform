namespace BMWDALInterfacesAndDTOs.DTOs;

public class CreateClubDTO
{
    public int ClubId { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    
    public string? Land { get; set; }
    
    public int OwnerId { get; set; }
    
    
}
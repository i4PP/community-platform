using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class Clubdetails
{
    public int ClubId { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? Land { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<ClubMembership>? Members { get; set; }
    
    public Clubdetails(int clubId, string name, string desc, string land, DateTime createdAt, List<ClubMembership> members)
    {
        ClubId = clubId;
        Name = name;
        Desc = desc;
        Land = land;
        CreatedAt = createdAt;
        Members = members;
    }
    
    public Clubdetails(ClubDetailsDTO clubdetailsDto)
    {
        ClubId = clubdetailsDto.ClubId;
        Name = clubdetailsDto.Name;
        Desc = clubdetailsDto.Desc;
        Land = clubdetailsDto.Land;
        CreatedAt = clubdetailsDto.CreatedAt;
        Members = clubdetailsDto.Members?.Select(m => new ClubMembership(m)).ToList();
    }
    
    
}
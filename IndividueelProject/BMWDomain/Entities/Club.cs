using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class Club
{
    public int ClubId { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? Land { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Club(int clubId, string name, string desc, string land, DateTime createdAt)
    {
        ClubId = clubId;
        Name = name;
        Desc = desc;
        Land = land;
        CreatedAt = createdAt;
    }
    
    public Club(ClubDTO clubDto)
    {
        ClubId = clubDto.ClubId;
        Name = clubDto.Name;
        Desc = clubDto.Desc;
        Land = clubDto.Land;
        CreatedAt = clubDto.CreatedAt;
    }
    
    public ClubDTO ToClubDto()
    {
        return new ClubDTO()
        {
            ClubId = ClubId,
            Name = Name,
            Desc = Desc,
            Land = Land,
            CreatedAt = CreatedAt
        };
    }
    
}
using BMWDALInterfacesAndDTOs.DTOs;


namespace BMWDomain.Entities;

public class CreateClub
{
    public int ClubId { get; set; }
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? Land { get; set; }
    public int OwnerId { get; set; }
    
    
    public CreateClub(int clubId, string name, string desc, string land, int ownerId)
    {
        ClubId = clubId;
        Name = name;
        Desc = desc;
        Land = land;
        OwnerId = ownerId;
    }

    
    public CreateClub(CreateClubDTO createClubDto)
    {
        Name = createClubDto.Name;
        Desc = createClubDto.Desc;
        Land = createClubDto.Land;
        OwnerId = createClubDto.OwnerId;
    }
    
public CreateClubDTO ToDTO()
    {
        return new CreateClubDTO
        {
            Name = Name,
            Desc = Desc,
            Land = Land,
            OwnerId = OwnerId,
        };
    }
    
    
}
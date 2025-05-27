using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDALInterfacesAndDTOs.Interfaces;

public interface IClubRepository
{
    public void CreateClub(CreateClubDTO createClubDto);
    public void RegisterUserToClub(ClubMembershipDTO clubMembershipDto);
    public List<ClubDTO> GetUserClub(int userId);
    
    public ClubDetailsDTO GetClubDetail(int clubId);
    
    public void UpdateMembershipRole(ClubMembershipDTO clubMembershipDto);
    

}
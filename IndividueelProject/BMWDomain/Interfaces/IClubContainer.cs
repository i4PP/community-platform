using BMWDomain.Entities;


namespace BMWDomain.interfaces;

public interface IClubContainer
{
    public void CreateClub(CreateClub club);
    public void RegisterUserToClub(ClubMembership clubMembership);
    
    public List<Club> GetUserClub(int userId);
    
    public Clubdetails GetClubDetail(int clubId);
    
    public void UpdateMembershipRole(ClubMembership clubMembership);
    

}
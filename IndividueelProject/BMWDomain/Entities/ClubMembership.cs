using BMWDALInterfacesAndDTOs.DTOs;


namespace BMWDomain.Entities;


public class ClubMembership
{
    public int ClubId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    
    public ClubMembership(int clubId, int userId, int roleId)
    {
        ClubId = clubId;
        UserId = userId;
        RoleId = roleId;
    }
    
    public ClubMembership(ClubMembershipDTO clubMembershipDto)
    {
        ClubId = clubMembershipDto.ClubId;
        UserId = clubMembershipDto.UserId;
        RoleId = clubMembershipDto.RoleId;
    }
    
    public ClubMembershipDTO ToDTO()
    {
        return new ClubMembershipDTO
        {
            ClubId = ClubId,
            UserId = UserId,
            RoleId = RoleId
        };
    }
    
}
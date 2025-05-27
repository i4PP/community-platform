using BMWDomain.Entities;

namespace BMWDomain.interfaces;

public interface IClubRolesContainer
{
    public ClubRole GetRoleById(int roleId);
    
    public List<ClubRole> GetRoles();
    
}
using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDALInterfacesAndDTOs.Interfaces;

public interface IClubRolesRepository
{
    public ClubRoleDTO GetRoleById(int roleId);
    
    public List<ClubRoleDTO> GetRoles();
    
}
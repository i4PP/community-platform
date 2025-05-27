using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class ClubRole
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    
    public ClubRole(int roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }
    
    
    public ClubRole(ClubRoleDTO dto)
    {
        RoleId = dto.RoleId;
        RoleName = dto.RoleName ?? throw new ArgumentNullException(nameof(dto));
    }
    
    public ClubRoleDTO ToDTO()
    {
        return new ClubRoleDTO()
        {
            RoleId = RoleId,
            RoleName = RoleName
        };
    }
    

}
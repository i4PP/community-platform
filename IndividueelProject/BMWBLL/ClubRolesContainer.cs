using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;

namespace BMW.BLL;

public class ClubRolesContainer : IClubRolesContainer
{
    private readonly IClubRolesRepository _clubRolesRepository;
    
    public ClubRolesContainer(IClubRolesRepository clubRolesRepository)
    {
        this._clubRolesRepository = clubRolesRepository;
    }

    public ClubRole GetRoleById(int roleId)
    {
        try
        {
            var dto = _clubRolesRepository.GetRoleById(roleId);
            return new ClubRole(dto);
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting role details 501", e);
        }
        catch (Exception e)
        {
            throw new BllException("error while getting role details 500", e);
        }
        

    }
    
    public List<ClubRole> GetRoles()
    {
        try
        {
            var dtos = _clubRolesRepository.GetRoles();
            return dtos.Select(dto => new ClubRole(dto)).ToList();
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting role details 501", e);
        }

    }
    
}
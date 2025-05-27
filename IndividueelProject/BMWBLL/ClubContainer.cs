using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using FluentValidation;

namespace BMW.BLL;

public class ClubContainer : IClubContainer
{
    private readonly IClubRepository _clubRepository;
    private readonly IValidator<CreateClub> _clubValidator;
    

    public ClubContainer(IClubRepository clubRepository, IValidator<CreateClub> clubValidator)
    {
        this._clubRepository = clubRepository;
        this._clubValidator = clubValidator;
    }

    public void CreateClub(CreateClub club)
    {
        _clubValidator.ValidateAndThrow(club);
        try
        {

            var dto = club.ToDTO();
            _clubRepository.CreateClub(dto);
            club.ClubId = dto.ClubId;
            ClubMembership clubMembership = new ClubMembership(club.ClubId, club.OwnerId, 1);
            RegisterUserToClub(clubMembership);
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1" ,e );
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while creating club 501", e);
        }


    }

    public void RegisterUserToClub(ClubMembership clubMembership)
    {
        try
        {
            _clubRepository.RegisterUserToClub(clubMembership.ToDTO());
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DuplicateEntryException e)
        {
            throw new DuplicateEntryException("User is already registered to this club 539", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while registering user to club 501", e);
        }
    }
    
    public List<Club> GetUserClub(int userId)
    {
        try
        {
            var dtos = _clubRepository.GetUserClub(userId);
            return dtos.Select(dto => new Club(dto)).ToList();

        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting user clubs 501", e);
        }


    }
    
    public Clubdetails GetClubDetail(int clubId)
    {
        try
        {
            var dto = _clubRepository.GetClubDetail(clubId);
            return new Clubdetails(dto);
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting club details 501", e);
        }


    }
    
    public void UpdateMembershipRole(ClubMembership clubMembership)
    {
        try
        {
            _clubRepository.UpdateMembershipRole(clubMembership.ToDTO());
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while updating membership role 501", e);
        }


    }
    

    
}
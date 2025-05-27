using FluentValidation;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMW.BLL;

public class InviteCodeContainer : IInviteCodeContainer
{
    private readonly IInviteCodeRepository _inviteCodeRepository;
    private readonly IValidator<InviteCode> _inviteCodeValidator;

    public InviteCodeContainer(IInviteCodeRepository inviteCodeRepository, IValidator<InviteCode> inviteCodeValidator)
    {
        this._inviteCodeRepository = inviteCodeRepository;
        this._inviteCodeValidator = inviteCodeValidator;
    }
    


    public void CreateInviteCode(InviteCode inviteCode)
    {            
        _inviteCodeValidator.ValidateAndThrow(inviteCode);
        try
        {
            InviteCodeDTO dto = inviteCode.ToDTO();
            _inviteCodeRepository.CreateInviteCode(dto);
            inviteCode.Code = dto.Code;
            inviteCode.ClubId = dto.ClubId;

        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting invitecode details 501", e);
        }
    }
    
    
    
    public InviteCode GetInviteCodeById(string inviteCode)
    {
        try
        {
            var result = _inviteCodeRepository.GetInviteCodeByCode(inviteCode);
            return new InviteCode(result);
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting invitecode details 501", e);
        }
        catch (Exception e)
        {
            throw new BllException("error while getting invitecode details 501", e);
        }

 

        
    }
    
    public List<InviteCode> GetInviteCodesByClubId(int clubId)
    {
        try
        {
            List<InviteCode> inviteCodes = new List<InviteCode>();
            var result = _inviteCodeRepository.GetInviteCodesByClubId(clubId);
            foreach (var inviteCode in result)
            {
                inviteCodes.Add(new InviteCode(inviteCode));
            }
            return inviteCodes;
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting invitecode details 501", e);
        }
        catch (Exception e)
        {
            throw new BllException("error while getting invitecode details 501", e);
        }

    }



    
}
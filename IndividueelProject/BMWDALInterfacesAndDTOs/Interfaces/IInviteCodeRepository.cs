using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDALInterfacesAndDTOs.Interfaces;

public interface IInviteCodeRepository
{
    
    public InviteCodeDTO GetInviteCodeByCode(string inviteCode);
    
    public void CreateInviteCode(InviteCodeDTO inviteCode);
    
    public List<InviteCodeDTO> GetInviteCodesByClubId(int clubId);
    
    
    
}
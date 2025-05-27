using BMWDomain.Entities;
namespace BMWDomain.interfaces;

public interface IInviteCodeContainer
{
    void CreateInviteCode(InviteCode inviteCode);
    InviteCode GetInviteCodeById(string inviteCode);
    
    public List<InviteCode> GetInviteCodesByClubId(int clubId);
    
}
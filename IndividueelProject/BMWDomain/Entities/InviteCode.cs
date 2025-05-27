using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class InviteCode
{
    
    public int Id { get; set; }
    public string? Code { get; set; }
    public int ClubId { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int MaxUses { get; set; }
    
    public int UserId { get; set; }
    
    
    
    
    public InviteCode(string code, int clubId, DateTime expirationDate, int maxUses, int id, int userId)
    {
        Code = code;
        ClubId = clubId;
        ExpirationDate = expirationDate;
        MaxUses = maxUses;
        Id = id;
        UserId = userId;
    }


    public InviteCode(InviteCodeDTO dto)
    {
        Code = dto.Code;
        ClubId = dto.ClubId;
        ExpirationDate = dto.ExpirationDate;
        MaxUses = dto.MaxUses;
        Id = dto.Id;
        UserId = dto.UserId;
    }
    
    public InviteCodeDTO ToDTO()
    {
        return new InviteCodeDTO
        {
            Code = Code,
            ClubId = ClubId,
            ExpirationDate = ExpirationDate,
            MaxUses = MaxUses,
            Id = Id,
            UserId = UserId
        };
    }

    
}
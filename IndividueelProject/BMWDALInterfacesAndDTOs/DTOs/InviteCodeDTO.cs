namespace BMWDALInterfacesAndDTOs.DTOs;

public class InviteCodeDTO
{

    public int Id { get; set; }
    public string? Code { get; set; }
    public int ClubId { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int MaxUses { get; set; }
    
    public int UserId { get; set; }
    
}
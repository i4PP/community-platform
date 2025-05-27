using BMWDALInterfacesAndDTOs.DTOs;
namespace BMWDALInterfacesAndDTOs.Interfaces;

public interface IChatRepository
{
    void CreateMessage(MessageDTO dto);
    List<MessageDTO> GetClubMessages(int clubId);
    
}
using BMWDomain.Entities;

namespace BMWDomain.interfaces;

public interface IChatContainer
{
    void SendMessage(Message message);
    List<Message> GetClubMessages(int clubId);
    
}
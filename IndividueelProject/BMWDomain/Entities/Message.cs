using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class Message
{
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public int ClubId { get; set; }
    public string? Content { get; set; }
    
    
    public Message(int messageId, int userId, int clubId, string content)
    {
        MessageId = messageId;
        UserId = userId;
        ClubId = clubId;
        Content = content;
    }

    public Message(MessageDTO messageDto)
    {
        MessageId = messageDto.MessageId;
        UserId = messageDto.UserId;
        ClubId = messageDto.ClubId;
        Content = messageDto.Content;
        
    }
    
    public MessageDTO ToMessageDto()
    {
        return new MessageDTO()
        {
            MessageId = MessageId,
            UserId = UserId,
            ClubId = ClubId,
            Content = Content
        };
    }
}
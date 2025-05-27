using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using FluentValidation;


namespace BMW.BLL;

public class ChatContainer : IChatContainer
{
    private readonly IChatRepository _chatRepository;
    private readonly IValidator<Message> _messageValidator;
    
    public ChatContainer(IChatRepository chatRepository, IValidator<Message> messageValidator)
    {
        this._chatRepository = chatRepository;
        this._messageValidator = messageValidator;
    }

    public void SendMessage(Message message)
    {
        _messageValidator.ValidateAndThrow(message);
        try
        {
            var dto = message.ToMessageDto();
            _chatRepository.CreateMessage(dto);
            message.MessageId = dto.MessageId;
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while creating message 501", e);
        }

    }

    public List<Message> GetClubMessages(int clubId)
    {
        try
        {
            var dtos = _chatRepository.GetClubMessages(clubId);
            return dtos.Select(dto => new Message(dto)).ToList();

        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting messages 501", e);
        }


    }
    

    
}
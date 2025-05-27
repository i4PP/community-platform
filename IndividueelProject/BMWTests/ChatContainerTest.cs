using BMWDomain.Entities;
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using Moq;
using BMW.BLL;
using FluentValidation;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMWTests;
[TestClass]
public class ChatContainerTest
{
    
    private  Mock<IChatRepository> _chatRepository = null! ;
    private  ChatContainer _chatContainer = null!;
    private  IValidator<Message> _messageValidator = null!;

    
    [TestInitialize]
    public void Initialize()
    {
        _chatRepository = new Mock<IChatRepository>();
        _messageValidator = new MessageValidator();
        _chatContainer = new ChatContainer(_chatRepository.Object,_messageValidator);
    }

    [DataRow("test", 1, 1, true)] // validTestCase
    [DataRow("", 1, 1, false)] // invalidTestCase no message
    [DataRow("test", 0, 1, false)] // invalidTestCase no userId
    [DataRow("test", 1, 0, false)] // invalidTestCase no clubId
    [TestMethod]
    public void SendMessage(string message, int userId, int clubId, bool expected)
    {
        // Arrange
        var messageEntity = new Message(0, userId, clubId, message);
        _chatRepository.Setup(x => x.CreateMessage(It.IsAny<MessageDTO>()))
            .Callback((MessageDTO messageDto) => messageDto.MessageId = 1);
        
        if (expected)
        {
            _chatContainer.SendMessage(messageEntity);
            Assert.AreEqual(1, messageEntity.MessageId);
        }
        else
        {
            // Act
            Action act = () => _chatContainer.SendMessage(messageEntity);

            // Assert
            Assert.ThrowsException<ValidationException>(act);
        }

    }
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }

    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void SendMessageThrowsException(Exception exception)
    {
        // Arrange
        var messageEntity = new Message(0, 1, 1, "test");
        _chatRepository.Setup(x => x.CreateMessage(It.IsAny<MessageDTO>()))
            .Throws(exception);

        // Act
        Action act = () => _chatContainer.SendMessage(messageEntity);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetClubMessagesThrowsException(Exception exception)
    {
        // Arrange
        _chatRepository.Setup(x => x.GetClubMessages(1))
            .Throws(exception);

        // Act
        Action act = () => _chatContainer.GetClubMessages(1);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    

    
    
    [DataTestMethod]
    [DataRow(1, true)] // validTestCase
    [DataRow(0, false)] // invalidTestCase no clubId
    [DataRow(2, false)] // clubId not found
    public void GetClubMessages(int clubId, bool expectedResult)
    {
        // Arrange
        var messageDtos = new List<MessageDTO>
        {
            new MessageDTO
            {
                ClubId = 1,
                MessageId = 1,
                UserId = 1,
                Content = "test"
            },
            new MessageDTO
            {
                ClubId = 1,
                MessageId = 2,
                UserId = 1,
                Content = "test"
            }
        };

        if (clubId == 1)
        {
            _chatRepository.Setup(x => x.GetClubMessages(clubId)).Returns(messageDtos);
        }
        else
        {
            _chatRepository.Setup(x => x.GetClubMessages(clubId)).Returns(new List<MessageDTO>());
        }

        // Act
        var messages = _chatContainer.GetClubMessages(clubId);

        // Assert
        if (expectedResult)
        {
            Assert.AreEqual(2, messages.Count);
            Assert.AreEqual(messages[0].MessageId, messageDtos[0].MessageId);
            Assert.AreEqual(messages[1].MessageId, messageDtos[1].MessageId);
            Assert.AreEqual(messages[0].UserId, messageDtos[0].UserId);
            Assert.AreEqual(messages[1].UserId, messageDtos[1].UserId);
            Assert.AreEqual(messages[0].Content, messageDtos[0].Content);
            Assert.AreEqual(messages[1].Content, messageDtos[1].Content);
        }
        else
        {
            Assert.AreEqual(0, messages.Count);
        }
    }
}



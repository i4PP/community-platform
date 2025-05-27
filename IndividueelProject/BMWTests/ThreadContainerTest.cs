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
public class ThreadContainerTest
{

    private  Mock<IDiscussionThreadRepository> _mockThreadRepository = null!;
    private DiscussionThreadContainer _threadContainer = null!;
    private ThreadValidator _threadValidator = null!;


    [TestInitialize]
    public void Initialize()
    {
        _mockThreadRepository = new Mock<IDiscussionThreadRepository>();
        _threadValidator = new ThreadValidator();
        _threadContainer = new DiscussionThreadContainer(_mockThreadRepository.Object, _threadValidator);

    }
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }





    [DataRow("Title", "Text", 1, 1, false, true)]
    [DataRow(" ", "Text", 1, 1, false, false)] // Title is empty
    [DataRow("1234567891011aemoidajiowrjqwoijiqwonrquoinqewuiqen", "Text", 1, 1, false, false)] // Title is longer than 34 characters
    [DataRow("Title", "longtext", 1, 1, false, false)] // Text is longer than 5000 characters
    [DataRow("Title", " ", 1, 1, false, false)] // Text is empty
    [DataRow("Title", "Text", null, 1, false, false)] // OwnerId is empty
    [DataRow("Title", "Text", 1, null, false, false)] // TopicId is empty
    [TestMethod]
    public void CreateThread_CheckMethodCalling(string title, string text, int ownerId, int topicId, bool isEdited,
        bool expected)
    {
        // Arrange
        if (text == "longtext")
        {
            text = new string('a', 5001);
        }

        var thread = new DiscussionThread(0, title, text, ownerId, DateTimeOffset.Now, topicId, isEdited);
        _mockThreadRepository.Setup(x => x.CreateThread(It.IsAny<DiscussionThreadDTO>()))
            .Callback((DiscussionThreadDTO dto) => dto.ThreadId = 1);
        
        
        
        // Assert
        if (expected)
        {
            _threadContainer.CreateThread(thread);
            Assert.AreEqual(1, thread.ThreadId);
        }
        else
        {
            Action act = () => _threadContainer.CreateThread(thread);
            _mockThreadRepository.Verify(x => x.CreateThread(It.IsAny<DiscussionThreadDTO>()), Times.Never);
            Assert.ThrowsException<ValidationException>(act);

        }
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void CreateThreadException(Exception exception)
    {
        // Arrange
        var thread = new DiscussionThread(0, "Title", "Text", 1, DateTimeOffset.Now, 1, false);
        _mockThreadRepository.Setup(x => x.CreateThread(It.IsAny<DiscussionThreadDTO>())).Throws(exception);
        
        // Act
        Action act = () => _threadContainer.CreateThread(thread);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void GetAllThreads_CheckMethodCalling()
    {
        // Arrange
        List<DiscussionThreadDTO> threads = new List<DiscussionThreadDTO>();
        threads.Add(new DiscussionThreadDTO
        {
            ThreadId = 1,
            Title = "Title",
            Text = "Text",
            OwnerId = 1,
            CreatedAt = DateTimeOffset.Now,
            TopicId = 1,
            IsEdited = false
            
        });
        _mockThreadRepository.Setup(x => x.GetAllThreads()).Returns(threads);
        
        // Act
        _threadContainer.GetAllThreads();
        
        // Assert
        _mockThreadRepository.Verify(x => x.GetAllThreads(), Times.Once);
        Assert.AreEqual(1, threads.Count);
        Assert.AreEqual(1, threads[0].ThreadId);
        Assert.AreEqual("Title", threads[0].Title);
        Assert.AreEqual("Text", threads[0].Text);
        Assert.AreEqual(1, threads[0].OwnerId);
        Assert.AreEqual(1, threads[0].TopicId);
        Assert.IsFalse(threads[0].IsEdited);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetAllThreadsException(Exception exception)
    {
        // Arrange
        _mockThreadRepository.Setup(x => x.GetAllThreads()).Throws(exception);
        
        // Act
        Action act = () => _threadContainer.GetAllThreads();
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [DataRow(1, true)]
    [DataRow(2, false)]
    [TestMethod]
    public void GetThreadById_CheckMethodCalling(int id, bool expected)
    {
        // Arrange
        var thread = new DiscussionThreadDTO
        {
            ThreadId = 1,
            Title = "Title",
            Text = "Text",
            OwnerId = 1,
            CreatedAt = DateTimeOffset.Now,
            TopicId = 1,
            IsEdited = false
        };
        _mockThreadRepository.Setup(x => x.GetThreadById(1)).Returns(thread);
        

        
        // Assert
        if (expected)
        {
            var result = _threadContainer.GetThreadById(id);
            _mockThreadRepository.Verify(x => x.GetThreadById(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(1, result.ThreadId);
            Assert.AreEqual("Title", result.Title);
            Assert.AreEqual("Text", result.Text);
            Assert.AreEqual(1, result.OwnerId);
            Assert.AreEqual(1, result.TopicId);
            Assert.IsFalse(result.IsEdited);
        }
        else
        {
            Action act = () => _threadContainer.GetThreadById(id);
            Assert.ThrowsException<BllException>(act);
        }

    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetThreadByIdException(Exception exception)
    {
        // Arrange
        _mockThreadRepository.Setup(x => x.GetThreadById(1)).Throws(exception);
        
        // Act
        Action act = () => _threadContainer.GetThreadById(1);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void DeleteThreadById_CheckMethodCalling()
    {
        // Arrange
        _mockThreadRepository.Setup(x => x.DeleteThreadById(1));
        
        // Act
        _threadContainer.DeleteThreadById(1);
        
        // Assert
        _mockThreadRepository.Verify(x => x.DeleteThreadById(It.IsAny<int>()), Times.Once);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void DeleteThreadByIdException(Exception exception)
    {
        // Arrange
        _mockThreadRepository.Setup(x => x.DeleteThreadById(1)).Throws(exception);
        
        // Act
        Action act = () => _threadContainer.DeleteThreadById(1);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void EditThread_CheckMethodCalling()
    {
        // Arrange
        var thread = new DiscussionThread(1, "Title", "Text", 1, DateTimeOffset.Now, 1, false);
        _mockThreadRepository.Setup(x => x.EditThread(It.IsAny<DiscussionThreadDTO>())).Callback((DiscussionThreadDTO dto) => dto.IsEdited = true);
        
        // Act
        _threadContainer.EditThread(thread);
        
        // Assert
        _mockThreadRepository.Verify(x => x.EditThread(It.IsAny<DiscussionThreadDTO>()), Times.Once);
        Assert.IsTrue(thread.IsEdited);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void EditThreadException(Exception exception)
    {
        // Arrange
        var thread = new DiscussionThread(1, "Title", "Text", 1, DateTimeOffset.Now, 1, false);
        _mockThreadRepository.Setup(x => x.EditThread(It.IsAny<DiscussionThreadDTO>())).Throws(exception);
        
        // Act
        Action act = () => _threadContainer.EditThread(thread);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void GetThreadsByUserId_CheckMethodCalling()
    {
        // Arrange
        List<DiscussionThreadDTO> threads = new List<DiscussionThreadDTO>();
        threads.Add(new DiscussionThreadDTO
        {
            ThreadId = 1,
            Title = "Title",
            Text = "Text",
            OwnerId = 1,
            CreatedAt = DateTimeOffset.Now,
            TopicId = 1,
            IsEdited = false
            
        });
        _mockThreadRepository.Setup(x => x.GetThreadsByUserId(1)).Returns(threads);
        
        // Act
        _threadContainer.GetThreadsByUserId(1);
        
        // Assert
        _mockThreadRepository.Verify(x => x.GetThreadsByUserId(It.IsAny<int>()), Times.Once);
        Assert.AreEqual(1, threads.Count);
        Assert.AreEqual(1, threads[0].ThreadId);
        Assert.AreEqual("Title", threads[0].Title);
        Assert.AreEqual("Text", threads[0].Text);
        Assert.AreEqual(1, threads[0].OwnerId);
        Assert.AreEqual(1, threads[0].TopicId);
        Assert.IsFalse(threads[0].IsEdited);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetThreadsByUserIdException(Exception exception)
    {
        // Arrange
        _mockThreadRepository.Setup(x => x.GetThreadsByUserId(1)).Throws(exception);
        
        // Act
        Action act = () => _threadContainer.GetThreadsByUserId(1);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    
}
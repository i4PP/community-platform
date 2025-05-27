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
public class CommentContainerTest
{
    private Mock<ICommentRepository> _commentRepository = null!;
    private CommentContainer _commentContainer = null!;
    private IValidator<Comment> _commentValidator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _commentRepository = new Mock<ICommentRepository>();
        _commentValidator = new CommentValidator();
        _commentContainer = new CommentContainer(_commentRepository.Object, _commentValidator);
    }
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }

    
    
    [DataRow("test", 1, 1, true)] // validTestCase
    [DataRow("", 1, 1, false)] // invalidTestCase no text
    [DataRow("test", 0, 1, false)] // invalidTestCase no ownerId
    [DataRow("test", 1, 0, false)] // invalidTestCase no threadId
    [TestMethod]
    public void CreateComment(string text, int ownerId, int threadId, bool expected)
    {
        // Arrange
        var comment = new Comment(0, text, DateTime.Now, ownerId, threadId, 0);
        _commentRepository.Setup(x => x.CreateComment(It.IsAny<CommentDTO>())).Callback((CommentDTO commentDto) => commentDto.CommentId = 1);
        
        if (expected)
        {
            // Act
            _commentContainer.CreateComment(comment);
            
            // Assert
            _commentRepository.Verify(x => x.CreateComment(It.IsAny<CommentDTO>()), Times.Once);
            Assert.AreEqual(1, comment.CommentId);
            
        }
        else
        {
            Action act = () => _commentContainer.CreateComment(comment);
            Assert.ThrowsException<ValidationException>(act);

        }
        
        
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void CreateCommentException(Exception exception)
    {
        // Arrange
        var comment = new Comment(0, "test", DateTime.Now, 1, 1, 0);
        _commentRepository.Setup(x => x.CreateComment(It.IsAny<CommentDTO>())).Throws(exception);
        
        // Act
        Action act = () => _commentContainer.CreateComment(comment);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [DataRow(1, true)]//thread exists
    [DataRow(2, false)]//thread does not exist
    [TestMethod]
    public void GetCommentsByThreadId(int threadId, bool expected)
    {
        // Arrange
        RootAndChilderenCommentsDTO comments = new RootAndChilderenCommentsDTO
        {
            RootComments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    CommentId = 1,
                    Text = "test",
                    CreatedAt = DateTime.Now,
                    OwnerId = 1,
                    ThreadId = 1,
                    ParentId = 0
                }
            },
            ChildComments = new List<CommentDTO>
            {
                new CommentDTO
                {
                    CommentId = 2,
                    Text = "test",
                    CreatedAt = DateTime.Now,
                    OwnerId = 1,
                    ThreadId = 1,
                    ParentId = 1
                },
                new CommentDTO
                {
                    CommentId = 3,
                    Text = "test",
                    CreatedAt = DateTime.Now,
                    OwnerId = 1,
                    ThreadId = 1,
                    ParentId = 1
                }
            }

        };

        _commentRepository.Setup(x => x.GetCommentsByThreadId(1)).Returns(comments);
        
        if (expected)
        {
            // Act
            var result = _commentContainer.GetCommentsByThreadId(threadId);
            
            // Assert
            _commentRepository.Verify(x => x.GetCommentsByThreadId(threadId), Times.Once);
            Assert.AreEqual(1, result.RootComments.Count);
            Assert.AreEqual(2, result.ChildComments.Count);
        }
        else
        {
            Action act = () => _commentContainer.GetCommentsByThreadId(threadId);
            Assert.ThrowsException<BllException>(act);
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetCommentsByThreadIdException(Exception exception)
    {
        // Arrange
        _commentRepository.Setup(x => x.GetCommentsByThreadId(1)).Throws(exception);
        
        // Act
        Action act = () => _commentContainer.GetCommentsByThreadId(1);
        
        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
}
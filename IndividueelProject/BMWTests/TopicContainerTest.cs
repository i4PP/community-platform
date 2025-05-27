
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using Moq;
using BMW.BLL;
using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDomain.Exceptions;

namespace BMWTests;

[TestClass]
public class TopicContainerTest
{
    private Mock<ITopicRepository> mockTopicRepository = null!;
    private TopicContainer topicContainer = null!;
    
    [TestInitialize]
    public void Initialize()
    {
        mockTopicRepository = new Mock<ITopicRepository>();
        topicContainer = new TopicContainer(mockTopicRepository.Object);
    }
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }
    
    [TestMethod]
    public void GetAllTopicTest()
    {
        // Arrange
        var topicDTO = new TopicDTO
        {
            TopicId = 1,
            Name = "TestTopic",
        };
        mockTopicRepository.Setup(x => x.GetAllTopic()).Returns(new List<TopicDTO> { topicDTO });

        // Act
        var result = topicContainer.GetAllTopic();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].TopicId);
        Assert.AreEqual("TestTopic", result[0].Name);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetAllTopicExceptionTest(Exception exception)
    {
        // Arrange
        mockTopicRepository.Setup(x => x.GetAllTopic()).Throws(exception);

        // Act
        Action act = () => topicContainer.GetAllTopic();

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void GetTopicByIdTest()
    {
        // Arrange
        var topicDTO = new TopicDTO
        {
            TopicId = 1,
            Name = "TestTopic",
        };
        mockTopicRepository.Setup(x => x.GetTopicById(1)).Returns(topicDTO);

        // Act
        var result = topicContainer.GetTopicById(1);

        // Assert
        Assert.AreEqual(1, result.TopicId);
        Assert.AreEqual("TestTopic", result.Name);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetTopicByIdExceptionTest(Exception exception)
    {
        // Arrange
        mockTopicRepository.Setup(x => x.GetTopicById(1)).Throws(exception);

        // Act
        Action act = () => topicContainer.GetTopicById(1);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }




}

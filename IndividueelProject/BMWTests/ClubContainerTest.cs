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
public class ClubContainerTest
{
    private Mock<IClubRepository> _clubRepository = null!;
    private ClubContainer _clubContainer = null!;
    private IValidator<CreateClub> _clubValidator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _clubRepository = new Mock<IClubRepository>();
        _clubValidator = new ClubValidator();
        _clubContainer = new ClubContainer(_clubRepository.Object, _clubValidator);
    }
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }
    
    public static IEnumerable<object[]> GetExceptionTestDataWithDuplicateEntryException()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new DuplicateEntryException("User is already registered to this club 539") };
    }
    

    [DataRow("test", "test", 1, "test", true)] // validTestCase
    [DataRow("", "test", 1, "test", false)] // invalidTestCase no name
    [DataRow("test", "", 1, "test", false)] // invalidTestCase no desc
    [DataRow("test", "test", 0, "test", false)] // invalidTestCase no ownerId
    [DataRow("test", "test", 1, "", false)] // invalidTestCase no land
    [TestMethod]
    public void CreateClub(string name, string desc, int ownerId, string land, bool expected)
    {
        // Arrange
        var club = new CreateClub(0, name, desc, land, ownerId);
        _clubRepository.Setup(x => x.CreateClub(It.IsAny<CreateClubDTO>()))
            .Callback((CreateClubDTO clubDto) => clubDto.ClubId = 1);

        if (expected)
        {
            _clubContainer.CreateClub(club);
            Assert.AreEqual(1, club.ClubId);

        }
        else
        {
            // Act
            Action act = () => _clubContainer.CreateClub(club);

            // Assert
            Assert.ThrowsException<ValidationException>(act);

        }
    }
    


    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void CreateClub_exception(Exception  exception)
    {
        // Arrange
        var club = new CreateClub(0, "test", "test", "test", 1);
        _clubRepository.Setup(x => x.CreateClub(It.IsAny<CreateClubDTO>()))
            .Throws(exception);

        // Act
        Action act = () => _clubContainer.CreateClub(club);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    
    [TestMethod]
    public void RegisterUserToClub()
    {
        // Arrange
        var clubMembership = new ClubMembership(1, 1, 1);
        _clubRepository.Setup(x => x.RegisterUserToClub(It.IsAny<ClubMembershipDTO>()));



        // Act
        _clubContainer.RegisterUserToClub(clubMembership);
        
        // Assert
        _clubRepository.Verify(x => x.RegisterUserToClub(It.IsAny<ClubMembershipDTO>()), Times.Once);
            
        

    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestDataWithDuplicateEntryException), DynamicDataSourceType.Method)]
    public void RegisterUserToClub_exception(Exception exception)
    {
        // Arrange
        var clubMembership = new ClubMembership(1, 1, 1);
        _clubRepository.Setup(x => x.RegisterUserToClub(It.IsAny<ClubMembershipDTO>()))
            .Throws(exception);

        // Act
        Action act = () => _clubContainer.RegisterUserToClub(clubMembership);

        // Assert
        if (exception is DuplicateEntryException)
        {
            Assert.ThrowsException<DuplicateEntryException>(act);
        }
        else
        {
            Assert.ThrowsException<DataBaseException>(act);
        }

    }
    
    
    [TestMethod]
    public void GetUserClub_successfully()
    {
        // Arrange
        var dtos = new List<ClubDTO>()
        {
            new ClubDTO()
            {
                ClubId = 1,
                Name = "test",
                Desc = "test",
                Land = "test",
            }
        };
        _clubRepository.Setup(x => x.GetUserClub(1)).Returns(dtos);

        // Act
        var result = _clubContainer.GetUserClub(1);

        // Assert
        Assert.AreEqual(dtos.Count, result.Count);
        Assert.AreEqual(dtos[0].ClubId, result[0].ClubId);
        Assert.AreEqual(dtos[0].Name, result[0].Name);
        Assert.AreEqual(dtos[0].Desc, result[0].Desc);
        Assert.AreEqual(dtos[0].Land, result[0].Land);
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetUserClub_exception(Exception  exception)
    {
        // Arrange
        _clubRepository.Setup(x => x.GetUserClub(1)).Throws(exception);

        // Act
        Action act = () => _clubContainer.GetUserClub(1);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void GetClubDetail_successfully()
    {
        var members = new List<ClubMembershipDTO>()
        {
            new ClubMembershipDTO()
            {
                ClubId = 1,
                UserId = 1,
                RoleId = 1
            }

        };
        // Arrange
        var dto = new ClubDetailsDTO()
        {
            ClubId = 1,
            Name = "test",
            Desc = "test",
            Land = "test",
            CreatedAt = DateTime.Now,
            Members = members
        };
        _clubRepository.Setup(x => x.GetClubDetail(1)).Returns(dto);

        // Act
        var result = _clubContainer.GetClubDetail(1);
        // Assert
        Assert.AreEqual(dto.ClubId, result.ClubId);
        Assert.AreEqual(dto.Name, result.Name);
        Assert.AreEqual(dto.Desc, result.Desc);
        Assert.AreEqual(dto.Land, result.Land);
        Assert.AreEqual(dto.CreatedAt, result.CreatedAt);
        Assert.AreEqual(dto.Members.Count, result.Members!.Count);
        Assert.AreEqual(dto.Members[0].ClubId, result.Members[0].ClubId);
        Assert.AreEqual(dto.Members[0].UserId, result.Members[0].UserId);
        Assert.AreEqual(dto.Members[0].RoleId, result.Members[0].RoleId);

    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetClubDetail_exception(Exception  exception)
    {
        // Arrange
        _clubRepository.Setup(x => x.GetClubDetail(1)).Throws(exception);

        // Act
        Action act = () => _clubContainer.GetClubDetail(1);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void UpdateMembershipRole()
    {
        // Arrange
        var clubMembership = new ClubMembership(1, 1, 1);
        _clubRepository.Setup(x => x.UpdateMembershipRole(It.IsAny<ClubMembershipDTO>()));

        // Act
        _clubContainer.UpdateMembershipRole(clubMembership);
        
        // Assert
        _clubRepository.Verify(x => x.UpdateMembershipRole(It.IsAny<ClubMembershipDTO>()), Times.Once);
    }
    
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void UpdateMembershipRole_exception(Exception  exception)
    {
        // Arrange
        var clubMembership = new ClubMembership(1, 1, 1);
        _clubRepository.Setup(x => x.UpdateMembershipRole(It.IsAny<ClubMembershipDTO>()))
            .Throws(exception);

        // Act
        Action act = () => _clubContainer.UpdateMembershipRole(clubMembership);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }

}
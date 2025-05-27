
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using Moq;
using BMW.BLL;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMWTests;

[TestClass]
public class ClubRolesContainerTest
{
    private Mock<IClubRolesRepository> _clubRolesRepository = null!;
    private ClubRolesContainer _clubRolesContainer = null!;
    
    [TestInitialize]
    public void Initialize()
    {
        _clubRolesRepository = new Mock<IClubRolesRepository>();
        _clubRolesContainer = new ClubRolesContainer(_clubRolesRepository.Object);
    }
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }

    [DataRow(1, true)] // validTestCase
    [DataRow(2, false)] // invalidTestCase
    [TestMethod]
    public void GetRoleById(int roleId, bool expected)
    {
        // Arrange
        var dto = new ClubRoleDTO()
        {
            RoleId = 1,
            RoleName = "test"
        };
        _clubRolesRepository.Setup(x => x.GetRoleById(1)).Returns(dto);

        if (expected)
        {
            // Act
            var result = _clubRolesContainer.GetRoleById(roleId);

            // Assert
            Assert.AreEqual(dto.RoleId, result.RoleId);
            Assert.AreEqual(dto.RoleName, result.RoleName);
        }
        else
        {
            // Act
            Action act = () => _clubRolesContainer.GetRoleById(roleId);

            // Assert
            Assert.ThrowsException<BllException>(act);
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetRoleByIdExceptions(Exception exception)
    {
        // Arrange
        _clubRolesRepository.Setup(x => x.GetRoleById(1)).Throws(exception);

        // Act
        Action act = () => _clubRolesContainer.GetRoleById(1);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [TestMethod]
    public void GetRoles()
    {
        // Arrange
        var dtos = new List<ClubRoleDTO>()
        {
            new ClubRoleDTO()
            {
                RoleId = 1,
                RoleName = "test"
            },
            new ClubRoleDTO()
            {
                RoleId = 2,
                RoleName = "test2"
            }
        };
        _clubRolesRepository.Setup(x => x.GetRoles()).Returns(dtos);

        // Act
        var result = _clubRolesContainer.GetRoles();

        // Assert
        Assert.AreEqual(dtos.Count, result.Count);
        for (int i = 0; i < dtos.Count; i++)
        {
            Assert.AreEqual(dtos[i].RoleId, result[i].RoleId);
            Assert.AreEqual(dtos[i].RoleName, result[i].RoleName);
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetRolesExceptions(Exception exception)
    {
        // Arrange
        _clubRolesRepository.Setup(x => x.GetRoles()).Throws(exception);

        // Act
        Action act = () => _clubRolesContainer.GetRoles();

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
  
    
}
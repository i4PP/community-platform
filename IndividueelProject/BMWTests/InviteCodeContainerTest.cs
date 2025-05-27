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
public class InviteCodeContainerTest
{
    private Mock<IInviteCodeRepository> _inviteCodeRepository = null!;
    private InvteCodeValidator _inviteCodeValidator = null!;
    private InviteCodeContainer _inviteCodeContainer = null!;
    
    public static IEnumerable<object[]> GetExceptionTestData()
    {
        yield return new object[] { new DalException("error while creating message 501") };
        yield return new object[] { new NetworkException("Network-related error occurred while connecting to the database. -1") };
        yield return new object[] { new DalException("error while creating message 501") };
    }

    [TestInitialize]
    public void Initialize()
    {
        _inviteCodeRepository = new Mock<IInviteCodeRepository>();
        _inviteCodeValidator = new InvteCodeValidator();
        _inviteCodeContainer = new InviteCodeContainer(_inviteCodeRepository.Object, _inviteCodeValidator);
    }
    
    [DataRow("empty", 1, 1, 1, 1, true)] // valid
    [DataRow("empty", 0, 1, 1, 1, false)]// ClubId is required
    [DataRow("empty", 1, 0, 1, 1, false)]// maxUses is required
    [TestMethod]
    public void CreateInviteCod(string code, int clubId, int maxUses, int id, int userId,bool expected)
    {
        // Arrange
        InviteCode inviteCode = new InviteCode(code, clubId, DateTime.Now, maxUses, id, userId);
        

        
        _inviteCodeRepository.Setup(x => x.CreateInviteCode(It.IsAny<InviteCodeDTO>()))
            .Callback((InviteCodeDTO dto) =>
            {
                dto.Id = 1;
                dto.Code = "AD1KM12";
            });

        
        // Act
        if (expected)
        {
            _inviteCodeContainer.CreateInviteCode(inviteCode);
            Assert.AreEqual("AD1KM12", inviteCode.Code);
            Assert.AreEqual(1, inviteCode.Id);
        }
        else
        {
            Action act = () => _inviteCodeContainer.CreateInviteCode(inviteCode);
            Assert.ThrowsException<ValidationException>(act);
        }

    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void CreateInviteCodeException(Exception exception)
    {
        // Arrange
        InviteCode inviteCode = new InviteCode("empty", 1, DateTime.Now, 1, 1, 1);
        
        _inviteCodeRepository.Setup(x => x.CreateInviteCode(It.IsAny<InviteCodeDTO>()))
            .Throws(exception);

        // Act
        Action act = () => _inviteCodeContainer.CreateInviteCode(inviteCode);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    
    
    [DataRow("AD1KM12",true)] // valid
    [DataRow("AD1KM11", false)]// ClubId is required
    [TestMethod]
    public void GetInviteCodeById(string code, bool expected)
    {
        // Arrange
        InviteCodeDTO inviteCodeDTO = new InviteCodeDTO
        {
            Code = code,
            ClubId = 1,
            ExpirationDate = DateTime.Now,
            MaxUses = 1,
            Id = 1,
            UserId = 1
        };
        
        _inviteCodeRepository.Setup(x => x.GetInviteCodeByCode("AD1KM12"))
            .Returns(inviteCodeDTO);

        
        // Act
        if (expected)
        {
            InviteCode inviteCode = _inviteCodeContainer.GetInviteCodeById(code);
            Assert.AreEqual(code, inviteCode.Code);
            Assert.AreEqual(1, inviteCode.Id);
        }
        else
        {
            Action act = () => _inviteCodeContainer.GetInviteCodeById(code);
            Assert.ThrowsException<BllException>(act);
        }

    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetInviteCodeByIdException(Exception exception)
    {
        // Arrange
        InviteCodeDTO inviteCodeDTO = new InviteCodeDTO
        {
            Code = "AD1KM12",
            ClubId = 1,
            ExpirationDate = DateTime.Now,
            MaxUses = 1,
            Id = 1,
            UserId = 1
        };
        
        _inviteCodeRepository.Setup(x => x.GetInviteCodeByCode("AD1KM12"))
            .Throws(exception);

        // Act
        Action act = () => _inviteCodeContainer.GetInviteCodeById("AD1KM12");

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    [DataRow(1, true)] // valid
    [DataRow(0, false)]// ClubId is required
    [TestMethod]
    public void GetInviteCodesByClubId(int clubId, bool expected)
    {
        // Arrange
        List<InviteCodeDTO> inviteCodeDTOs = new List<InviteCodeDTO>
        {
            new InviteCodeDTO
            {
                Code = "AD1KM12",
                ClubId = 1,
                ExpirationDate = DateTime.Now,
                MaxUses = 1,
                Id = 1,
                UserId = 1
            }
        };
        
        _inviteCodeRepository.Setup(x => x.GetInviteCodesByClubId(1))
            .Returns(inviteCodeDTOs);

        
        // Act
        if (expected)
        {
            List<InviteCode> inviteCodes = _inviteCodeContainer.GetInviteCodesByClubId(clubId);
            Assert.AreEqual(1, inviteCodes.Count);
            Assert.AreEqual("AD1KM12", inviteCodes[0].Code);
            Assert.AreEqual(1, inviteCodes[0].Id);
        }
        else
        {
            Action act = () => _inviteCodeContainer.GetInviteCodesByClubId(clubId);
            Assert.ThrowsException<BllException>(act);
        }

    }
    
    [TestMethod]
    [DynamicData(nameof(GetExceptionTestData), DynamicDataSourceType.Method)]
    public void GetInviteCodesByClubIdException(Exception exception)
    {
        // Arrange
        List<InviteCodeDTO> inviteCodeDTOs = new List<InviteCodeDTO>
        {
            new InviteCodeDTO
            {
                Code = "AD1KM12",
                ClubId = 1,
                ExpirationDate = DateTime.Now,
                MaxUses = 1,
                Id = 1,
                UserId = 1
            }
        };
        
        _inviteCodeRepository.Setup(x => x.GetInviteCodesByClubId(1))
            .Throws(exception);

        // Act
        Action act = () => _inviteCodeContainer.GetInviteCodesByClubId(1);

        // Assert
        Assert.ThrowsException<DataBaseException>(act);
    }
    
    


    
    
    
}
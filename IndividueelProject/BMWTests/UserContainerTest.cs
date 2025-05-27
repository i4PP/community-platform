using BMWDomain.Entities;
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.DTOs;
using BMW.BLL;
using BMWTests.MockRepos;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;

namespace BMWTests;

[TestClass]
public class UserContainerTest
{
    private UserContainer? _userContainer;
    private MockUserRepository? _userRepositoryMock;
    private UserValidator? _userValidator;
    private const string Salt = "cDuyoaMMY77aJfi6NB3PuPvlznMYgh!";
    
    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new MockUserRepository();
        _userValidator = new UserValidator();
        _userContainer = new UserContainer(_userRepositoryMock, _userValidator);
    }

    private static byte[] ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return bytes;
        }
    }
    [TestMethod]
    [DataRow("password123", true)]
    [DataRow("password123wrong", false)]
    public void LoginUser_Password_Test(string password, bool expected)
    {
        // Arrange
        var login = new Login("testuser", password, 0);

        string saltedPassword = Salt + "password123"; 
        var passwordHashed = ComputeSha256Hash(saltedPassword);
        _userRepositoryMock!.SetMockLoginData(new LoginDTO
        {
            Id = 1,
            User = "testuser",
            Password = passwordHashed
        });

        // Act
        Action act = () => _userContainer!.LoginUser(login);

        // Assert
        if (expected)
        {
            act(); 
            Assert.AreEqual(1, login.Id);
            Assert.AreEqual("testuser", login.User);
        }
        else
        {
            Assert.ThrowsException<BllException>(act);
        }
    }



    [TestMethod]
    [DataRow("Password123?#!A", true)]
    [DataRow("Aaa!1", false)] //shorter than 6
    [DataRow("Aaabbc1", false)] //no special character   
    [DataRow("aaabbc1", false)] //no uppercase
    [DataRow("Aaabbc", false)] //no number
    [DataRow(" ", false)]//empty
    public void RegisterUser_Test(string password, bool expected)
    {
        // Arrange
        var account = new Account(0, "TestUserUnit", password, "test@gmail,com");
         
        _userRepositoryMock!.SetMockAccountData(new AccountDTO
        {
            UserId = 1,
            Name = account.Name,
            Password = ComputeSha256Hash(Salt + password),
            Email = "test@gmail,com"
        });
        
        // Act
        Action act = () => _userContainer!.RegisterUser(account);
        
        
        // Assert
        if (expected)
        {
            act();
            Assert.AreEqual(1, account.UserId);
            Assert.AreEqual("TestUserUnit", account.Name);
            Assert.AreEqual("test@gmail,com", account.Email);
            Assert.AreEqual( password, account.Password);

        }
        else
        {
            Assert.ThrowsException<ValidationException>(act);

        }

    }
    
    [TestMethod]
    [DataRow(1, true)]
    [DataRow(2, false)]
    public void GetUserById_Test(int userId, bool expected)
    {
        // Arrange
        _userRepositoryMock!.SetMockUserData(new UserDTO
        {
            UserId = 1,
            Name = "TestUserUnit",
            Email = "test@gmail,com"
        });
        
        // Act
        Action act = () => _userContainer!.GetUserById(userId);
        
        // Assert
        if (expected)
        {
            act();
        }
        else
        {
            Assert.ThrowsException<BllException>(act);
        }
    }
    
        [TestMethod]
    public void GetUserById_NetworkException_Test()
    {
        // Arrange
        _userRepositoryMock!.SimulateNetworkException(true);

        // Act & Assert
        Assert.ThrowsException<DataBaseException>(() => _userContainer!.GetUserById(1));
    }

    [TestMethod]
    public void GetUserById_DalException_Test()
    {
        // Arrange
        _userRepositoryMock!.SimulateDalException(true);

        // Act & Assert
        Assert.ThrowsException<DataBaseException>(() => _userContainer!.GetUserById(1));
    }

    [TestMethod]
    public void RegisterUser_DuplicateEntryException_Test()
    {
        // Arrange
        var account = new Account(0, "TestUserUnit", "Password123?#!A", "test@gmail,com");
        _userRepositoryMock!.SimulateDuplicateEntryException(true);

        // Act & Assert
        Assert.ThrowsException<UsernameIsUsedException>(() => _userContainer!.RegisterUser(account));
    }

    [TestMethod]
    public void RegisterUser_NetworkException_Test()
    {
        // Arrange
        var account = new Account(0, "TestUserUnit", "Password123?#!A", "test@gmail,com");
        _userRepositoryMock!.SimulateNetworkException(true);

        // Act & Assert
        Assert.ThrowsException<DataBaseException>(() => _userContainer!.RegisterUser(account));
    }

    [TestMethod]
    public void LoginUser_InvalidPassword_Test()
    {
        // Arrange
        var login = new Login("testuser", "wrongpassword", 0);
        string saltedPassword = Salt + "password123"; 
        var passwordHashed = ComputeSha256Hash(saltedPassword);
        _userRepositoryMock!.SetMockLoginData(new LoginDTO
        {
            Id = 1,
            User = "testuser",
            Password = passwordHashed
        });

        // Act & Assert
        Assert.ThrowsException<BllException>(() => _userContainer!.LoginUser(login));
    }

    [TestMethod]
    public void LoginUser_NetworkException_Test()
    {
        // Arrange
        var login = new Login("testuser", "password123", 0);
        _userRepositoryMock!.SimulateNetworkException(true);

        // Act & Assert
        Assert.ThrowsException<DataBaseException>(() => _userContainer!.LoginUser(login));
    }

    [TestMethod]
    public void LoginUser_DalException_Test()
    {
        // Arrange
        var login = new Login("testuser", "password123", 0);
        _userRepositoryMock!.SimulateDalException(true);

        // Act & Assert
        Assert.ThrowsException<DataBaseException>(() => _userContainer!.LoginUser(login));
    }



}
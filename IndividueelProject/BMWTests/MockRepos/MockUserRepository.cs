using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDALInterfacesAndDTOs.Exceptions;

namespace BMWTests.MockRepos;

public class MockUserRepository : IUserRepository
{
    private LoginDTO? _mockLoginData;
    private AccountDTO? _mockAccountData;
    private UserDTO? _mockUserData;

    private bool _throwNetworkException;
    private bool _throwDalException;
    private bool _throwDuplicateEntryException;

    public void SetMockLoginData(LoginDTO loginData)
    {
        _mockLoginData = loginData;
    }
    public void SetMockAccountData(AccountDTO accountData)
    {
        _mockAccountData = accountData;
    }
    public void SetMockUserData(UserDTO userData)
    {
        _mockUserData = userData;
    }

    public void SimulateNetworkException(bool throwException)
    {
        _throwNetworkException = throwException;
    }

    public void SimulateDalException(bool throwException)
    {
        _throwDalException = throwException;
    }

    public void SimulateDuplicateEntryException(bool throwException)
    {
        _throwDuplicateEntryException = throwException;
    }

    public UserDTO GetUserById(int id)
    {
        if (_throwNetworkException)
        {
            throw new NetworkException("Network error");
        }

        if (_throwDalException)
        {
            throw new DalException("DAL error");
        }

        if (id == _mockUserData!.UserId)
        {
            return _mockUserData;
        }
        else
        {
            throw new BllException("User not found");
        }
    }

    public void RegisterUser(AccountDTO account)
    {
        if (_throwNetworkException)
        {
            throw new NetworkException("Network error");
        }

        if (_throwDalException)
        {
            throw new DalException("DAL error");
        }

        if (_throwDuplicateEntryException)
        {
            throw new DuplicateEntryException("Duplicate entry");
        }

        if (account.Name == _mockAccountData!.Name)
        {
            account.UserId = 1;
        }
        else
        {
            throw new BllException("User not found");
        }
    }

    public void GetUserByNameOrEmail(LoginDTO login)
    {
        if (_throwNetworkException)
        {
            throw new NetworkException("Network error");
        }

        if (_throwDalException)
        {
            throw new DalException("DAL error");
        }

        if (login.User == _mockLoginData!.User)
        {
            login.Id = _mockLoginData.Id;
            login.User = _mockLoginData.User;
            login.Password = _mockLoginData.Password;
        }
        else
        {
            throw new BllException("User not found");
        }
    }
}

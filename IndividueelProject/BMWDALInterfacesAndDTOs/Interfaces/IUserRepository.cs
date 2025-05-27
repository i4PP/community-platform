using BMWDALInterfacesAndDTOs.DTOs;


namespace BMWDALInterfacesAndDTOs.Interfaces;
    public interface IUserRepository
    {
        UserDTO GetUserById(int id);

        void RegisterUser(AccountDTO account);

        void GetUserByNameOrEmail(LoginDTO login);


    }


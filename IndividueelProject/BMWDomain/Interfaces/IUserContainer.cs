using BMWDomain.Entities;

namespace BMWDomain.interfaces;
    public interface IUserContainer
    {

        User GetUserById(int userId);

        void RegisterUser(Account account);
        
        void LoginUser(Login login);
    }


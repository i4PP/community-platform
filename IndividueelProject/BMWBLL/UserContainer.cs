using System.Text;
using System.Security.Cryptography;
using FluentValidation;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.DTOs;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using BMWDALInterfacesAndDTOs.Exceptions;


namespace BMW.BLL
{
    public class UserContainer : IUserContainer
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<Account> _userValidator;
        

        protected const string Salt = "cDuyoaMMY77aJfi6NB3PuPvlznMYgh!";

        public UserContainer(IUserRepository userRepository, IValidator<Account> userValidator)
        {
            this._userRepository = userRepository;
            this._userValidator = userValidator;
        }

        public User GetUserById(int userId)
        {
            try
            {
                var result = _userRepository.GetUserById(userId);
                User userResult = new User(result);
                return userResult;

            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the server -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting user details 501", e);
            }
            catch (Exception e)
            {
                throw new BllException("error while getting user details 500", e);
            }

        }

        public void RegisterUser(Account account)
        {                
            _userValidator.ValidateAndThrow(account);
            try
            {
                AccountDTO dto = account.ToDTO();
                string saltedPassword = Salt + account.Password;
                dto.Password = ComputeSha256Hash(saltedPassword);
                _userRepository.RegisterUser(dto);
                account.UserId = dto.UserId;

            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the server -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("error while getting user details 501", e);
            }
            catch (DuplicateEntryException e)
            {
                throw new UsernameIsUsedException("User is already registered 787", e);
            } 

            

        }

        public void LoginUser(Login login)
        {
            try
            {
                LoginDTO dto = login.ToDTO();
                string saltedPassword = Salt + login.Password;
                byte[] hashedPassword = ComputeSha256Hash(saltedPassword);
                _userRepository.GetUserByNameOrEmail(dto);
                if (!hashedPassword.SequenceEqual(dto.Password!))
                {
                    throw new InvalidPasswordException("Password is incorrect");
                }
                login.Id = dto.Id;

            }
            catch (NetworkException e)
            {
                throw new DataBaseException("Network-related error occurred while connecting to the server -1", e);
            }
            catch (DalException e)
            {
                throw new DataBaseException("Password or username is wrong", e);
            }
            catch (DuplicateEntryException e)
            {
                throw new UsernameIsUsedException("User is already registered 787", e);
            } 
            catch (InvalidPasswordException e)
            {
                throw new BllException("Password or username is wrong", e);
            }
        }   

        static byte[] ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

                return bytes;
            }
        }
    }
}

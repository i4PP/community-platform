using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

    public class Account
    {

        public int UserId { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public Account(int userId, string name, string password, string email)
        {
            UserId = userId;
            Name = name;
            Password = password;
            Email = email;
        }

        public AccountDTO ToDTO()
        {
            return new AccountDTO()
            {
                UserId = UserId,
                Name = Name,
                Email = Email,
            };
        }
        
    }


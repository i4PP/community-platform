using BMWDALInterfacesAndDTOs.DTOs;
namespace BMWDomain.Entities;

    public class Login
    {
        public int Id { get; set; }
        public string User { get; set; }

        public string Password { get; set; }

        public Login(string user, string password, int id)
        {
            User = user;
            Password = password;
            Id = id;
        }

        public LoginDTO ToDTO()
        {
            return new LoginDTO()
            {
                User = User,
            };
        }




    }


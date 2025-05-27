using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

    public class User
    {

        public int UserId { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }


        public User(int userId, string name, string email)
        {
            UserId = userId;
            Name = name;
            Email = email;
        }
        
        public User(UserDTO dto)
        {
            UserId = dto.UserId;
            Name = dto.Name ?? throw new ArgumentNullException(nameof(dto));
            Email = dto.Email ?? throw new ArgumentNullException(nameof(dto));
        }

        public UserDTO ToDTO()
        {
            return new UserDTO()
            {
                UserId = UserId,
                Name = Name,
                Email = Email,
            };
        }


    }


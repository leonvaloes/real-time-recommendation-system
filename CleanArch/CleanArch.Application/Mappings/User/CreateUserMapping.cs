using CleanArch.Application.DTOs;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Mappings;

public class CreateUserMapping : IMapping<User, CreateUserDTO>
{

        public CreateUserDTO ToDto(User entity)
        {
                return new CreateUserDTO
                {
                        Name = entity.Name,
                        Email = entity.Email,
                        Phone = entity.Phone
                };
        }

        public User ToEntity(CreateUserDTO dto)
        {
                return new User(dto.Name, dto.Email, dto.Phone);
        }

}
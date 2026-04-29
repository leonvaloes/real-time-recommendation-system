using UserAuth.CleanArch.Application.DTOs;
using UserAuth.CleanArch.Domain.Entities;

namespace UserAuth.CleanArch.Application.Mappings;

public class CreateUserMapping : IMapping<User, CreateUserDTO>
{

        public CreateUserDTO ToDto(User entity)
        {
                return new CreateUserDTO
                {
                        Email = entity.Email,
                        FirstName = entity.NaturalPerson?.FirstName ?? string.Empty,
                        LastName = entity.NaturalPerson?.LastName ?? string.Empty,
                        CPF = entity.NaturalPerson?.CPF?.Value,
                        Phone = entity.Phone
                };
        }

        public User ToEntity(CreateUserDTO dto)
        {
                var person = new NaturalPerson(dto.FirstName, dto.LastName, dto.CPF);
                return new User(dto.Email, dto.PasswordHash, person, dto.Phone);
        }

}

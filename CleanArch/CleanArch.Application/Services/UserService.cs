using CleanArch.Application.DTOs;
using CleanArch.Application.Mappings;
using CleanArch.Domain.Entities;

namespace CleanArchMvc.Application.Interfaces.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly CreateUserMapping createUserMapping;

    async Task<User> IUserService.CreateAsync(CreateUserDTO createUserDTO)
    {
        var createUserEntity =  createUserMapping.ToEntity(createUserDTO);
        await _userRepository.CreateAsync(createUserEntity);
        return createUserEntity;
    }

    Task<List<User>> IUserService.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    Task<User> IUserService.GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    Task<User> IUserService.GetByIdAsync(int? id)
    {
        throw new NotImplementedException();
    }

    Task<User> IUserService.RemoveByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    Task<User> IUserService.UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }
}

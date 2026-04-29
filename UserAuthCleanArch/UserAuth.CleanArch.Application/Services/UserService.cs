using UserAuth.CleanArch.Application.DTOs;
using UserAuth.CleanArch.Application.Interfaces;
using UserAuth.CleanArch.Application.Mappings;
using UserAuth.CleanArch.Domain.Entities;
using UserAuth.CleanArch.Domain.Interfaces;

namespace UserAuth.CleanArch.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly CreateUserMapping _createUserMapping;

    public UserService(IUserRepository userRepository, CreateUserMapping createUserMapping)
    {
        _userRepository = userRepository;
        _createUserMapping = createUserMapping;
    }

    public async Task<User> CreateAsync(CreateUserDTO createUserDTO)
    {
        var createUserEntity = _createUserMapping.ToEntity(createUserDTO);
        await _userRepository.CreateAsync(createUserEntity);
        return createUserEntity;
    }

    public Task<List<User>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _userRepository.GetByEmailAsync(email);
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _userRepository.GetByIdAsync(id);
    }

    public Task<User?> RemoveByIdAsync(Guid id)
    {
        return _userRepository.RemoveByIdAsync(id);
    }

    public Task<User> UpdateAsync(User user)
    {
        return _userRepository.UpdateAsync(user);
    }
}

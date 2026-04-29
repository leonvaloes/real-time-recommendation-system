using CleanArch.Application.DTOs;
using CleanArch.Application.Interfaces;
using CleanArch.Application.Mappings;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;

namespace CleanArch.Application.Services;

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

    public Task<User> GetByEmailAsync(string email)
    {
        return _userRepository.GetByEmailAsync(email);
    }

    public Task<User> GetByIdAsync(int? id)
    {
        return _userRepository.GetByIdAsync(id);
    }

    public Task<User> RemoveByIdAsync(int id)
    {
        return _userRepository.RemoveByIdAsync(id);
    }

    public Task<User> UpdateAsync(User user)
    {
        return _userRepository.UpdateAsync(user);
    }
}

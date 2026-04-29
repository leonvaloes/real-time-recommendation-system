using UserAuth.CleanArch.Application.DTOs;
using UserAuth.CleanArch.Domain.Entities;

namespace UserAuth.CleanArch.Application.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(CreateUserDTO user);
    Task<User> UpdateAsync(User user);
    Task<User?> RemoveByIdAsync(Guid id);
}

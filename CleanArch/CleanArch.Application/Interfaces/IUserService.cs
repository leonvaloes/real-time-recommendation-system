using CleanArch.Application.DTOs;
using CleanArch.Domain.Entities;

namespace CleanArchMvc.Application.Interfaces;

public interface IUserService
{
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(int? id);
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateAsync(CreateUserDTO user);
        Task<User> UpdateAsync(User user);
        Task<User> RemoveByIdAsync(int id);
}

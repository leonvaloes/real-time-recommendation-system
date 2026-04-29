using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using CleanArch.Infra.Data.Context;
using MongoDB.Driver;

namespace CleanArch.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoContext _mongoContext;

    public UserRepository(MongoContext mongoContext)
    {
        _mongoContext = mongoContext;
    }


    public async Task<User> CreateAsync(User user)
    {
        try
        {
            await _mongoContext.Users.InsertOneAsync(user);
            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public async Task<List<User>> GetAllAsync()
    {
        try
        {
            var sort = Builders<User>.Sort.Ascending(u => u.Email);
            var allUsers = await _mongoContext.Users.Find(_ => true).Sort(sort).ToListAsync();
            return allUsers;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            var normalizedEmail = email.ToLowerInvariant();
            return await _mongoContext.Users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _mongoContext.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task<User?> RemoveByIdAsync(Guid id)
    {
        try
        {
            var user = await GetByIdAsync(id);
            await _mongoContext.Users.DeleteOneAsync(u => u.Id == id);
            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        try
        {
            await _mongoContext.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}

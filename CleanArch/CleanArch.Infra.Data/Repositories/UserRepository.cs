using CleanArch.Domain.Entities;
using CleanArch.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CleanArch.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private MongoContext _mongoContext;
    public UserRepository (MongoContext mongoContext)
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
            var sort = Builders<User>.Sort.Ascending(u => u.Id);
            var allUsers = await _mongoContext.Users.Find(_ => true).Sort(sort).ToListAsync();
            return allUsers;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(int? id)
    {
        try
        {
            var sort = Builders<User>.Sort.Ascending(u => u.Id);
            var user = _mongoContext.Users.Find(u => u.Id == id).Sort(sort).FirstOrDefaultAsync();
            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public Task<User> RemoveByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

}
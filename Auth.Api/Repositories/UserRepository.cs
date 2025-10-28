using Auth.Api.Models;
using Auth.Api.Services;
using MongoDB.Driver;

namespace Auth.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        _users = db.GetCollection<User>("Users");
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await _users.Find(u => u.Phone == phone).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }
}
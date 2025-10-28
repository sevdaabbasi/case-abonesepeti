using MongoDB.Driver;
using Auth.Api.Models;
using Auth.Api.Repositories;
using Auth.Api.Services;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    public UserRepository(MongoSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var db = client.GetDatabase(settings.DatabaseName);
        _users = db.GetCollection<User>("users");
    }

    public async Task<User?> GetByPhoneAsync(string phone) =>
        await _users.Find(u => u.Phone == phone).FirstOrDefaultAsync();

    public async Task CreateAsync(User user) =>
        await _users.InsertOneAsync(user);

    public async Task UpdateAsync(User user) =>
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken) =>
        await _users.Find(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken)).FirstOrDefaultAsync();
}
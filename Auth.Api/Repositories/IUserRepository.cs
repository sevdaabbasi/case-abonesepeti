using Auth.Api.Models;

namespace Auth.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByPhoneAsync(string phone);
    Task CreateAsync(User user);
}
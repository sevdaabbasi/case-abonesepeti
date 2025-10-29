using Auth.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByPhoneAsync(string phone);
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    
    

}
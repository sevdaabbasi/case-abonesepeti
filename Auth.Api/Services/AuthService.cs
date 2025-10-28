using Auth.Api.Models;
using Auth.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Auth.Api.Services;

public class AuthService
{
    private readonly IUserRepository _repo;
    private readonly JwtSettings _jwt;

    public AuthService(IUserRepository repo, IOptions<JwtSettings> jwtOptions)
    {
        _repo = repo;
        _jwt = jwtOptions.Value;
    }

    
    
    public async Task<(bool Success, string Message, User? User)> RegisterAsync(RegisterRequest req)
    {
        if (req.Password != req.ConfirmPassword)
            return (false, "Parolalar eşleşmiyor.", null);

        var existing = await _repo.GetByPhoneAsync(req.Phone);
        if (existing != null)
            return (false, "Bu telefon numarası ile zaten kayıt var.", null);

        if (!Enum.TryParse<Role>(req.Role, true, out var role))
            role = Role.User;

        var user = new User
        {
            Phone = req.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = role
        };

        await _repo.CreateAsync(user);
        return (true, "Kayıt başarılı.", user);
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(LoginRequest req)
    {
        var user = await _repo.GetByPhoneAsync(req.Phone);
        if (user == null) return (false, "Kullanıcı bulunamadı.", null);

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return (false, "Parola yanlış.", null);

        return (true, "Giriş başarılı.", user);
    }

    // Basit doğrulama — her istekte gönderilen phone+password ile kontrol
    public async Task<User?> ValidateCredentialsAsync(string phone, string password)
    {
        var user = await _repo.GetByPhoneAsync(phone);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        return user;
    }
}
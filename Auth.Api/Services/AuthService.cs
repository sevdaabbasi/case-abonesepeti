using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Auth.Api.Models;
using Auth.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

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

    private string GenerateAccessToken(User user, out DateTime expiresAt)
    {
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(_jwt.Key);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? ""),
            new Claim("Phone", user.Phone),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var now = DateTime.UtcNow;
        expiresAt = now.AddMinutes(_jwt.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    // Helper: güvenli random refresh token oluştur
    private RefreshToken GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);
        return new RefreshToken
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public async Task<(bool Success, string Message, User? User, AuthTokens tokens)> RegisterAsync(RegisterRequest req)
    {
        if (req.Password != req.ConfirmPassword)
            return (false, "Parolalar eşleşmiyor.", null, null);

        var existing = await _repo.GetByPhoneAsync(req.Phone);
        if (existing != null)
            return (false, "Bu telefon numarası ile zaten kayıt var.", null, null);

        if (!Enum.TryParse<Role>(req.Role, true, out var role))
            role = Role.User;

        var user = new User
        {
            Phone = req.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = role
        };
        
        // İlk refresh token
        var refresh = GenerateRefreshToken();
        user.RefreshTokens.Add(refresh);


        await _repo.CreateAsync(user);
        
        // Access token oluştur
        var accessToken = GenerateAccessToken(user, out var atExpires);
        var tokens = new AuthTokens(accessToken, refresh.Token, atExpires, refresh.ExpiresAt);
        
        return (true, "Kayıt başarılı.", user, tokens);
    }
    
    

    public async Task<(bool Success, string Message, User? User, AuthTokens? Tokens)> LoginAsync(LoginRequest req)
    {
        var user = await _repo.GetByPhoneAsync(req.Phone);
        if (user == null) return (false, "Kullanıcı bulunamadı.", null, null);

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return (false, "Parola yanlış.", null, null);

        // Yeni refresh token oluştur
        var refresh = GenerateRefreshToken();
        user.RefreshTokens.Add(refresh);

        // DB'de güncelle
        await _repo.UpdateAsync(user);

        var accessToken = GenerateAccessToken(user, out var atExpires);
        var tokens = new AuthTokens(accessToken, refresh.Token, atExpires, refresh.ExpiresAt);

        return (true, "Giriş başarılı.", user, tokens);
    }
    
    public async Task<(bool Success, string Message, AuthTokens? Tokens)> RefreshAsync(RefreshRequest req)
    {
        // Kullanıcıyı refresh token üzerinden bul
        var user = await _repo.GetByPhoneAsync(req.Phone);
        if (user == null) return (false, "Kullanıcı bulunamadı.", null);

        var existingRefresh = user.RefreshTokens.FirstOrDefault(rt => rt.Token == req.RefreshToken);
        if (existingRefresh == null) return (false, "Refresh token bulunamadı.", null);
        if (!existingRefresh.IsActive) return (false, "Refresh token geçersiz veya süresi dolmuş.", null);

        // Yeni refresh token oluştur ve eskiyi iptal et (rotate)
        var newRefresh = GenerateRefreshToken();
        existingRefresh.RevokedAt = DateTime.UtcNow;
        existingRefresh.ReplacedByToken = newRefresh.Token;
        user.RefreshTokens.Add(newRefresh);

        // DB güncelle
        await _repo.UpdateAsync(user);

        // Yeni access token
        var accessToken = GenerateAccessToken(user, out var atExpires);
        var tokens = new AuthTokens(accessToken, newRefresh.Token, atExpires, newRefresh.ExpiresAt);

        return (true, "Token yenilendi.", tokens);
    }
    
    
    // Basit doğrulama — her istekte gönderilen phone+password ile kontrol
    public async Task<User?> ValidateCredentialsAsync(string phone, string password)
    {
        var user = await _repo.GetByPhoneAsync(phone);
        if (user == null) return null;
        //return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        return user;
    }
}


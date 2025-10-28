namespace Auth.Api.Models;

public record RegisterRequest(string Phone, string Password, string ConfirmPassword, string Role);
public record LoginRequest(string Phone, string Password);
public record RefreshRequest(string Phone, string RefreshToken);
public record AuthTokens(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);


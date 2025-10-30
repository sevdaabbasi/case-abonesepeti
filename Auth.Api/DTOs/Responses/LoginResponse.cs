namespace Auth.Api.Dtos.Responses;

public record LoginResponse(
    string AccessToken, 
    string RefreshToken, 
    DateTime AccessTokenExpiresAt, 
    DateTime RefreshTokenExpiresAt);
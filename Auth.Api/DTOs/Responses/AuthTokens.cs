namespace Auth.Api.Dtos.Responses;

public record AuthTokens(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);
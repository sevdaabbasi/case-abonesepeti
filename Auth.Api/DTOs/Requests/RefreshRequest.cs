namespace Auth.Api.Dtos.Requests;

public record RefreshRequest(string AccessToken, string RefreshToken);
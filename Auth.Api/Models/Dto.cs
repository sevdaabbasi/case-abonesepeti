namespace Auth.Api.Models;

public record RegisterRequest(string Phone, string Password, string ConfirmPassword, string Role);
public record LoginRequest(string Phone, string Password);
public record AuthResponse(string Phone, string Role, string Message);

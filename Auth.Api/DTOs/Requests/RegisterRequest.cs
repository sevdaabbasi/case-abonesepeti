namespace Auth.Api.Dtos.Requests;

public record RegisterRequest
{
    public string Phone { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
    public string Role { get; init; } = "User"; 
}
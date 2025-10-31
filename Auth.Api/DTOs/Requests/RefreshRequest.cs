namespace Auth.Api.Dtos.Requests;

public class RefreshRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
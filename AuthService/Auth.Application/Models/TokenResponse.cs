using Shared.Contracts;

namespace Auth.Application.Models;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserDto User { get; set; }
}
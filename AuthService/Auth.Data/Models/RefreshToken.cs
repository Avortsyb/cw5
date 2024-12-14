namespace Auth.Data.Models;
public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }
}


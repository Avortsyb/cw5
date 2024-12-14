using System.Text.Json.Serialization;

namespace UserManagement.Data.Entities;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }

    [JsonIgnore]
    public ICollection<UserGroup>? UserGroups { get; set; } = new List<UserGroup>();
    [JsonIgnore]
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

}

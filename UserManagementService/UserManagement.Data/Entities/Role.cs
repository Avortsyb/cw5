using System.Text.Json.Serialization;

namespace UserManagement.Data.Entities;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    [JsonIgnore] public ICollection<UserRole> UserRoles { get; set; }
}
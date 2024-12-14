using System.Text.Json.Serialization;

namespace UserManagement.Data.Entities;

public class Group
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    [JsonIgnore] public ICollection<UserGroup> UserGroups { get; set; }
}
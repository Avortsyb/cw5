
using UserManagement.Data.Entities;

namespace UserManagement.Data.Repositories;

public interface IGroupRepository
{
    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<Group> GetGroupByIdAsync(int groupId);
    Task<Group?> GetGroupByNameAsync(string groupName);
    Task<IEnumerable<User>> GetUsersByGroupIdAsync(int groupId);
    Task AddGroupAsync(Group group);
    Task UpdateGroupAsync(Group group);
    Task DeleteGroupAsync(int groupId);
}
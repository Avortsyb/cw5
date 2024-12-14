using UserManagement.Data.Entities;

namespace UserManagement.Application.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<Group>> GetAllGroupsAsync();
        Task<Group> GetGroupByIdAsync(int groupId);
        Task<IEnumerable<User>> GetUsersByGroupIdAsync(int groupId);
        Task AddGroupAsync(Group group);
        Task UpdateGroupAsync(int groupId, string groupName);
        Task DeleteGroupAsync(int groupId);
    }
}

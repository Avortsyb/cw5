using UserManagement.Data.Entities;

namespace UserManagement.Application.Interfaces {
    public interface IUserService {
        Task<User?> CreateUserAsync(string username, string email, string password,
            List<int> roleIds, List<int>? groupIds = null);

        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId);
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> UpdateUserAsync(int userId, string? username, string? email);
        Task DeleteUserAsync(int userId);
        Task UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> ValidatePasswordAsync(string username, string password);
        Task AddRolesToUserAsync(int userId, List<int> roleIds);
        Task AddGroupsToUserAsync(int userId, List<int> groupIds);
        Task RemoveRolesFromUserAsync(int userId, List<int> roleIds);
        Task RemoveGroupsFromUserAsync(int userId, List<int> groupIds);
    }
}
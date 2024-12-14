using UserManagement.Data.Entities;

namespace UserManagement.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId);
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);

    Task<List<User>> GetAllUsersAsync();
    Task CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(User user);

    Task DeleteUserAsync(int userId);
}
using UserManagement.Data.Entities;

namespace UserManagement.Data.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<Role> GetRoleByIdAsync(int roleId);
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId);
    Task AddRoleAsync(Role role);
    Task UpdateRoleAsync(Role role);
    Task DeleteRoleAsync(int roleId);
}
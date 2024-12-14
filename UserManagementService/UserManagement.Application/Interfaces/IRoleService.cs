using UserManagement.Data.Entities;

namespace UserManagement.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int roleId);
        Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId);
        Task AddRoleAsync(Role role);
        Task UpdateRoleAsync(int roleId, string roleName);
        Task DeleteRoleAsync(int roleId);
    }
}

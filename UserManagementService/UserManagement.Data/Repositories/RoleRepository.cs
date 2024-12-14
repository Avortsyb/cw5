using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;

namespace UserManagement.Data.Repositories;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await context.Roles.ToListAsync();
    }

    public async Task<Role> GetRoleByIdAsync(int roleId)
    {
        return await context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == roleId) ?? throw new InvalidOperationException("Role not found.");
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId)
    {
        return await context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.User)
            .ToListAsync();
    }

    public async Task AddRoleAsync(Role role)
    {
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRoleAsync(Role role)
    {
        context.Roles.Update(role);
        await context.SaveChangesAsync();
    }

    public async Task DeleteRoleAsync(int roleId)
    {
        var role = await context.Roles.FindAsync(roleId);
        if (role == null)
            throw new InvalidOperationException("Role not found.");

        context.Roles.Remove(role);
        await context.SaveChangesAsync();
    }
}
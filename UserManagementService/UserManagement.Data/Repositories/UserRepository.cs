using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;

namespace UserManagement.Data.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository {
    public async Task<User?> GetUserByIdAsync(int userId) {
        return await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.UserGroups)
            .ThenInclude(ug => ug.Group)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }


    public async Task<User?> GetUserByUsernameAsync(string username) {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email) {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId) {
        return await context.UserGroups
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.Group)
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId) {
        return await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .ToListAsync();
    }


    public async Task<List<User>> GetAllUsersAsync() {
        return await context.Users
            .ToListAsync();
    }

    public async Task CreateUserAsync(User user) {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<User?> UpdateUserAsync(User user) {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }


    public async Task DeleteUserAsync(int userId) {
        var user = await context.Users.FindAsync(userId);
        if (user != null) {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
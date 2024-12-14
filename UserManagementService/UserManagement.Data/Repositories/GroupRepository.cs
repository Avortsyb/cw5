using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;

namespace UserManagement.Data.Repositories;

public class GroupRepository(ApplicationDbContext context) : IGroupRepository
{
    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await context.Groups.ToListAsync();
    }

    public async Task<Group> GetGroupByIdAsync(int groupId)
    {
        return await context.Groups
            .Include(g => g.UserGroups)
            .FirstOrDefaultAsync(g => g.GroupId == groupId) ?? throw new InvalidOperationException("Group not found.");
    }

    public async Task<Group?> GetGroupByNameAsync(string groupName)
    {
        return await context.Groups
            .FirstOrDefaultAsync(g => g.GroupName == groupName);
    }

    public async Task<IEnumerable<User>> GetUsersByGroupIdAsync(int groupId)
    {
        return await context.UserGroups
            .Where(ug => ug.GroupId == groupId)
            .Select(ug => ug.User)
            .ToListAsync();
    }

    public async Task AddGroupAsync(Group group)
    {
        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();
    }

    public async Task UpdateGroupAsync(Group group)
    {
        context.Groups.Update(group);
        await context.SaveChangesAsync();
    }

    public async Task DeleteGroupAsync(int groupId)
    {
        var group = await context.Groups.FindAsync(groupId);
        if (group == null)
            throw new InvalidOperationException("Group not found.");

        context.Groups.Remove(group);
        await context.SaveChangesAsync();
    }
}
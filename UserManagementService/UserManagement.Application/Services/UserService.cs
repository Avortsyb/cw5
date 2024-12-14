using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UserManagement.Application.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Data.Repositories;
using Group = UserManagement.Data.Entities.Group;

namespace UserManagement.Application.Services;

public class UserService(IUserRepository userRepository, IPasswordService passwordService, ILogger<UserService> logger)
    : IUserService
{
    public async Task<User?> CreateUserAsync(string username, string email, string password,
        List<int> roleIds, List<int>? groupIds)
    {
        var existingUsername = await userRepository.GetUserByUsernameAsync(username);
        var existingEmail = await userRepository.GetUserByEmailAsync(email);
        if (existingUsername != null || existingEmail != null)
            throw new InvalidOperationException("User or email already exists.");

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email)) {
            logger.LogWarning("User or email already exists for username: {Username} or email: {Email}", username, email);
            throw new ArgumentException("Username and email cannot be empty.");
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) {
            logger.LogWarning("Invalid email format: {Email}",  email);
            throw new ArgumentException("Invalid email format.");
        }

        var passwordHash = await passwordService.HashPasswordAsync(password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            UserGroups = groupIds?.Select(groupId => new UserGroup
            {
                GroupId = groupId,
            }).ToList(),

            UserRoles = roleIds.Select(roleId => new UserRole
            {
                RoleId = roleId
            }).ToList()
        };

        await userRepository.CreateUserAsync(user);
        logger.LogInformation("User with username: {Username} created successfully.", username);
        return user;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null) {
            logger.LogWarning("User not found with ID: {UserId}", userId);
            throw new InvalidOperationException("User not found.");
        }
        return user;
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) {
            logger.LogWarning("User not found with username: {Username}", username);
            throw new InvalidOperationException("User not found.");
        }
        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await userRepository.GetUserByEmailAsync(email);
        if (user == null) {
            logger.LogWarning("User not found with email: {Email}", email);
            throw new InvalidOperationException("Email not found.");
        }

        return user;
    }

    public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive number.", nameof(userId));

        return await userRepository.GetGroupsByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be a positive number.", nameof(userId));

        return await userRepository.GetRolesByUserIdAsync(userId);
    }


    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await userRepository.GetAllUsersAsync();
    }

    public async Task<User?> UpdateUserAsync(int userId, string? username, string? email)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            return null;
        }
        
        if (!string.IsNullOrEmpty(username) && user.Username != username)
        {
            var existingUser = await userRepository.GetUserByUsernameAsync(username);
            if (existingUser != null && existingUser.UserId != userId) {
                logger.LogWarning("Email is already taken for email: {Username}", username);
                throw new ArgumentException("Username is already taken.");
            }
            
            logger.LogInformation("Username updated to: {Username}", username);

            user.Username = username;
        }

        if (!string.IsNullOrEmpty(email) && user.Email != email)
        {
            var existingEmail = await userRepository.GetUserByEmailAsync(email);
            if (existingEmail != null && existingEmail.UserId != userId)
            {
                logger.LogWarning("Email is already taken for email: {Email}", email);
                throw new ArgumentException("Email is already taken.");
            }
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format.");

            user.Email = email;
            logger.LogInformation("Email updated to: {Email}", email);
        }
        logger.LogInformation("User with ID: {UserId} updated successfully.", userId);
        return await userRepository.UpdateUserAsync(user);
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null) {
            logger.LogWarning("User not found with ID: {UserId}", userId);
            throw new InvalidOperationException("User not found.");
        }

        await userRepository.DeleteUserAsync(userId);
        logger.LogInformation("User with ID: {UserId} deleted successfully.", userId);

    }

    public async Task UpdatePasswordAsync(int userId, string newPassword)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        user.PasswordHash = await passwordService.HashPasswordAsync(newPassword);
        await userRepository.UpdateUserAsync(user);
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        return await passwordService.ValidatePasswordAsync(password, user.PasswordHash);
    }
    
    public async Task AddRolesToUserAsync(int userId, List<int> roleIds)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User not found with ID: {UserId}", userId);
            throw new InvalidOperationException("User not found.");
        }

        foreach (var roleId in roleIds)
        {
            if (user.UserRoles.Any(ur => ur.RoleId == roleId))
            {
                logger.LogWarning("User with ID: {UserId} already has the role with ID: {RoleId}", userId, roleId);
                throw new InvalidOperationException($"User already has the role with ID {roleId}.");
            }

            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });
        }

        await userRepository.UpdateUserAsync(user);
        logger.LogInformation("User roles with ID: {UserId} updated successfully.", userId);
    }
    
    public async Task AddGroupsToUserAsync(int userId, List<int> groupIds)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User not found with ID: {UserId}", userId);
            throw new InvalidOperationException("User not found.");
        }

        foreach (var groupId in groupIds)
        {
            if (user.UserGroups != null && user.UserGroups.Any(ug => ug.GroupId == groupId))
            {
                logger.LogWarning("User with ID: {UserId} already has the group with ID: {RoleId}", userId, groupId);
                throw new InvalidOperationException($"User already has the group with ID {groupIds}.");
            }

            user.UserGroups?.Add(new UserGroup { UserId = userId, GroupId = groupId });
        }

        await userRepository.UpdateUserAsync(user);
        logger.LogInformation("User groups with ID: {UserId} updated successfully.", userId);
    }
    
    public async Task RemoveRolesFromUserAsync(int userId, List<int> roleIds)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (user.UserRoles == null || !user.UserRoles.Any())
            throw new InvalidOperationException("User has no roles assigned.");

        var rolesToRemove = user.UserRoles.Where(ur => roleIds.Contains(ur.RoleId)).ToList();

        if (!rolesToRemove.Any())
            throw new InvalidOperationException("None of the specified roles are assigned to the user.");

        if (user.UserRoles.Count - rolesToRemove.Count < 1)
            throw new InvalidOperationException("User cannot have less than one role.");

        foreach (var roleToRemove in rolesToRemove)
            user.UserRoles.Remove(roleToRemove);

        await userRepository.UpdateUserAsync(user);
        logger.LogInformation("User roles with ID: {UserId} updated successfully.", userId);
    }

    public async Task RemoveGroupsFromUserAsync(int userId, List<int> groupIds) {
        var user = await userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (user.UserGroups == null || !user.UserGroups.Any())
            throw new InvalidOperationException("User has no groups assigned.");

        var groupsToRemove = user.UserGroups.Where(ur => groupIds.Contains(ur.GroupId)).ToList();

        if (!groupsToRemove.Any())
            throw new InvalidOperationException("None of the specified groups are assigned to the user.");

        foreach (var groupToRemove in groupsToRemove)
            user.UserGroups.Remove(groupToRemove);

        await userRepository.UpdateUserAsync(user);
        logger.LogInformation("User groups with ID: {UserId} updated successfully.", userId);
    }
}
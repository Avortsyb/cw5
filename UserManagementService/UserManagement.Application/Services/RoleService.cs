using UserManagement.Application.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace UserManagement.Application.Services {
    public class RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        : IRoleService {
        public async Task<IEnumerable<Role>> GetAllRolesAsync() {
            var roles = await roleRepository.GetAllRolesAsync();
            var allRolesAsync = roles.ToList();
            if (roles == null || !allRolesAsync.Any()) {
                logger.LogWarning("No roles found in the system.");
                throw new InvalidOperationException("No roles found.");
            }

            logger.LogInformation("Fetched {RoleCount} roles.", allRolesAsync.Count);
            return allRolesAsync;
        }

        public async Task<Role> GetRoleByIdAsync(int roleId) {
            if (roleId <= 0) {
                logger.LogWarning("Invalid role ID provided: {RoleId}", roleId);
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
            }

            logger.LogInformation("Fetching role with ID: {RoleId}", roleId);
            var role = await roleRepository.GetRoleByIdAsync(roleId);
            if (role == null) {
                logger.LogWarning("Role with ID {RoleId} not found.", roleId);
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            return role;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId) {
            if (roleId <= 0) {
                logger.LogWarning("Invalid role ID provided: {RoleId}", roleId);
                throw new ArgumentException("Role ID must be a positive number.", nameof(roleId));
            }

            logger.LogInformation("Fetching users for role ID: {RoleId}", roleId);
            var users = await roleRepository.GetUsersByRoleIdAsync(roleId);
            return users;
        }

        public async Task AddRoleAsync(Role role) {
            if (role == null) {
                logger.LogWarning("Attempted to add a null role.");
                throw new ArgumentNullException(nameof(role), "Role cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(role.RoleName)) {
                logger.LogWarning("Role name is empty or null when adding new role.");
                throw new ArgumentException("Role name cannot be empty.", nameof(role.RoleName));
            }

            logger.LogInformation("Attempting to add role with name: {RoleName}", role.RoleName);
            var existingRoles = await roleRepository.GetAllRolesAsync();
            if (existingRoles.Any(r => r.RoleName.Equals(role.RoleName, StringComparison.OrdinalIgnoreCase))) {
                logger.LogWarning("Role with name '{RoleName}' already exists.", role.RoleName);
                throw new InvalidOperationException($"Role with name '{role.RoleName}' already exists.");
            }

            await roleRepository.AddRoleAsync(role);
            logger.LogInformation("Role with name '{RoleName}' added successfully.", role.RoleName);
        }

        public async Task UpdateRoleAsync(int roleId, string roleName) {
            if (roleId <= 0) {
                logger.LogWarning("Invalid role ID provided: {RoleId}", roleId);
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
            }

            if (string.IsNullOrWhiteSpace(roleName)) {
                logger.LogWarning("Role name is empty or null when updating role.");
                throw new ArgumentException("Role name cannot be empty.", nameof(roleName));
            }

            logger.LogInformation("Attempting to update role with ID: {RoleId}", roleId);
            var role = await roleRepository.GetRoleByIdAsync(roleId);
            if (role == null) {
                logger.LogWarning("Role with ID {RoleId} not found.", roleId);
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            var existingRole = await roleRepository.GetRoleByNameAsync(roleName);
            if (existingRole != null && existingRole.RoleId != roleId) {
                logger.LogWarning("Role with name '{RoleName}' already exists.", roleName);
                throw new InvalidOperationException($"A role with the name '{roleName}' already exists.");
            }

            role.RoleName = roleName;
            await roleRepository.UpdateRoleAsync(role);
            logger.LogInformation("Role with ID {RoleId} updated to new name: {RoleName}", roleId, roleName);
        }

        public async Task DeleteRoleAsync(int roleId) {
            if (roleId <= 0) {
                logger.LogWarning("Invalid role ID provided: {RoleId}", roleId);
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
            }

            logger.LogInformation("Attempting to delete role with ID: {RoleId}", roleId);
            var role = await roleRepository.GetRoleByIdAsync(roleId);
            if (role == null) {
                logger.LogWarning("Role with ID {RoleId} not found.", roleId);
                throw new InvalidOperationException($"Role with ID {roleId} not found.");
            }

            await roleRepository.DeleteRoleAsync(roleId);
            logger.LogInformation("Role with ID {RoleId} deleted successfully.", roleId);
        }
    }
}
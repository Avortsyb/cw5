using MassTransit;
using UserManagement.Application.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Data.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace UserManagement.Application.Services
{
    public class GroupService(IGroupRepository groupRepository, ILogger<GroupService> logger, IPublishEndpoint publishEndpoint)
        : IGroupService {
        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            logger.LogInformation("Fetching all groups.");
            var groups = await groupRepository.GetAllGroupsAsync();
            var allGroupsAsync = groups.ToList();
            if (!allGroupsAsync.Any())
            {
                logger.LogWarning("No groups found.");
                throw new InvalidOperationException("No groups found.");
            }

            logger.LogInformation("Fetched {GroupCount} groups.", allGroupsAsync.Count);
            return allGroupsAsync;
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            if (groupId <= 0)
            {
                logger.LogWarning("Invalid group ID provided: {GroupId}", groupId);
                throw new ArgumentException("Group ID must be a positive number.", nameof(groupId));
            }

            logger.LogInformation("Fetching group with ID: {GroupId}", groupId);
            var group = await groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                logger.LogWarning("Group with ID {GroupId} not found.", groupId);
                throw new InvalidOperationException($"Group with ID {groupId} not found.");
            }

            return group;
        }

        public async Task<IEnumerable<User>> GetUsersByGroupIdAsync(int groupId)
        {
            if (groupId <= 0)
            {
                logger.LogWarning("Invalid group ID provided: {GroupId}", groupId);
                throw new ArgumentException("Group ID must be a positive number.", nameof(groupId));
            }

            logger.LogInformation("Fetching users for group ID: {GroupId}", groupId);
            return await groupRepository.GetUsersByGroupIdAsync(groupId);
        }

        public async Task AddGroupAsync(Group group)
        {
            if (group == null)
            {
                logger.LogWarning("Attempted to add a null group.");
                throw new ArgumentNullException(nameof(group), "Group cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(group.GroupName))
            {
                logger.LogWarning("Group name is empty or null when adding new group.");
                throw new ArgumentException("Group name cannot be empty.", nameof(group.GroupName));
            }

            logger.LogInformation("Attempting to add group with name: {GroupName}", group.GroupName);
            var existingGroups = await groupRepository.GetAllGroupsAsync();
            if (existingGroups.Any(g => g.GroupName == group.GroupName))
            {
                logger.LogWarning("Group with name '{GroupName}' already exists.", group.GroupName);
                throw new InvalidOperationException("A group with the same name already exists.");
            }

            await groupRepository.AddGroupAsync(group);
            logger.LogInformation("Group with name '{GroupName}' added successfully.", group.GroupName);
        }

        public async Task UpdateGroupAsync(int groupId, string groupName)
        {
            if (groupId <= 0)
            {
                logger.LogWarning("Invalid group ID provided: {GroupId}", groupId);
                throw new ArgumentException("Group ID must be a positive number.", nameof(groupId));
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                logger.LogWarning("Group name is empty or null when updating group.");
                throw new ArgumentException("Group name cannot be empty.", nameof(groupName));
            }

            logger.LogInformation("Attempting to update group with ID: {GroupId}", groupId);
            var group = await groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                logger.LogWarning("Group with ID {GroupId} not found.", groupId);
                throw new InvalidOperationException($"Group with ID {groupId} not found.");
            }

            var existingGroup = await groupRepository.GetGroupByNameAsync(groupName);
            if (existingGroup != null && existingGroup.GroupId != groupId)
            {
                logger.LogWarning("Group with name '{GroupName}' already exists.", groupName);
                throw new InvalidOperationException($"A group with the name '{groupName}' already exists.");
            }

            group.GroupName = groupName;
            await groupRepository.UpdateGroupAsync(group);
            logger.LogInformation("Group with ID {GroupId} updated to new name: {GroupName}", groupId, groupName);
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            if (groupId <= 0)
            {
                logger.LogWarning("Invalid group ID provided: {GroupId}", groupId);
                throw new ArgumentException("Group ID must be a positive number.", nameof(groupId));
            }

            logger.LogInformation("Attempting to delete group with ID: {GroupId}", groupId);
            var group = await groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                logger.LogWarning("Group with ID {GroupId} not found.", groupId);
                throw new InvalidOperationException($"Group with ID {groupId} not found.");
            }

            await groupRepository.DeleteGroupAsync(groupId);
            logger.LogInformation("Group with ID {GroupId} deleted successfully.", groupId);

            await publishEndpoint.Publish(new GroupDeletedEvent { GroupId = groupId });
            logger.LogInformation("Published GroupDeletedEvent for GroupId: {GroupId}", groupId);
        }
    }
}

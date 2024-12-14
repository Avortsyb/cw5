using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace ContentManagement.Application.Services;

public class GroupValidationService(
    IRequestClient<CheckGroupRequest> requestClient,
    ILogger<GroupValidationService> logger) {
    public async Task<bool> ValidateGroupAsync(int groupId)
    {
        try
        {
            logger.LogInformation("Validating group with ID: {GroupId}", groupId);

            var response = await requestClient.GetResponse<CheckGroupResponse>(new CheckGroupRequest { GroupId = groupId });

            if (response.Message.Exists)
            {
                logger.LogInformation("Group validation succeeded: {GroupName}", response.Message.GroupName);
                return true;
            }
            else
            {
                logger.LogWarning("Group validation failed: {Error}", response.Message.Error);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating group with ID: {GroupId}", groupId);
            throw;
        }
    }
}

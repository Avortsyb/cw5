using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services;

public class CheckGroupRequestConsumer(IGroupService groupService, ILogger<CheckGroupRequestConsumer> logger)
    : IConsumer<CheckGroupRequest> {
    public async Task Consume(ConsumeContext<CheckGroupRequest> context) {
        var request = context.Message;

        try {
            logger.LogInformation("Received CheckGroupRequest for GroupId: {GroupId}", request.GroupId);

            var group = await groupService.GetGroupByIdAsync(request.GroupId); // Проверяем существование группы

            if (group != null) {
                await context.RespondAsync(new CheckGroupResponse {
                    Exists = true,
                    GroupName = group.GroupName
                });
                logger.LogInformation("Group found: {GroupName} (Id: {GroupId})", group.GroupName, request.GroupId);
            }
            else {
                await context.RespondAsync(new CheckGroupResponse {
                    Exists = false,
                    Error = "Group not found"
                });
                logger.LogWarning("Group not found for GroupId: {GroupId}", request.GroupId);
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error occurred while processing CheckGroupRequest for GroupId: {GroupId}",
                request.GroupId);
            await context.RespondAsync(new CheckGroupResponse {
                Exists = false,
                Error = ex.Message
            });
        }
    }
}
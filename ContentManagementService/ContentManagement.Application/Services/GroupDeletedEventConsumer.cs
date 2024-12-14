using ContentManagement.Data.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace ContentManagement.Application.Services;

public class GroupDeletedEventConsumer(IArticleRepository articleRepository, ILogger<GroupDeletedEventConsumer> logger)
    : IConsumer<GroupDeletedEvent> {
    public async Task Consume(ConsumeContext<GroupDeletedEvent> context)
    {
        var groupId = context.Message.GroupId;

        try
        {
            logger.LogInformation("Received GroupDeletedEvent for GroupId: {GroupId}", groupId);
            
            await articleRepository.RemoveGroupLinksAsync(groupId);
            logger.LogInformation("Removed all links to GroupId: {GroupId}", groupId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while processing GroupDeletedEvent for GroupId: {GroupId}", groupId);
            throw;
        }
    }
}
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services;

public class AuthRequestConsumer(IUserService userService, ILogger<AuthRequestConsumer> logger) : IConsumer<AuthRequest> {
    public async Task Consume(ConsumeContext<AuthRequest> context)
    {
        var request = context.Message;

        try
        {
            logger.LogInformation("Received AuthRequest for Username: {Username}", request.Username);
            if (!await userService.ValidatePasswordAsync(request.Username, request.Password))
            {
                logger.LogWarning("Invalid login attempt for Username: {Username}", request.Username);
                await context.RespondAsync(new AuthResponse
                {
                    Success = false,
                    Error = "Invalid username or password"
                });
                return;
            }

            var user = await userService.GetUserByUsernameAsync(request.Username);

            var roles = (await userService.GetRolesByUserIdAsync(user.UserId)).Select(role => role.RoleName).ToList();
            var groups = (await userService.GetGroupsByUserIdAsync(user.UserId)).Select(group => group.GroupName).ToList();

            var userDto = new UserDto
            {
                Id = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Roles = roles,
                Groups = groups
            };

            logger.LogInformation("Authentication successful for Username: {Username}", request.Username);

            await context.RespondAsync(new AuthResponse
            {
                Success = true,
                User = userDto
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing AuthRequest for Username: {Username}", request.Username);
            await context.RespondAsync(new AuthResponse
            {
                Success = false,
                Error = ex.Message
            });
        }
    }
}

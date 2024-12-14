using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Requests;
using UserManagement.Api.Responses;
using UserManagement.Application.Interfaces;
using UserManagement.Data.Entities;
using InvalidOperationException = System.InvalidOperationException;

namespace UserManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase {
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserRequest request) {
        if (request == null)
            return BadRequest("Request body cannot be null.");

        try {
            var user = await userService.CreateUserAsync(request.UserName, request.Email, request.Password,
                request.RoleIds, request.GroupIds);

            var roles = await userService.GetRolesByUserIdAsync(user.UserId);
            var groups = await userService.GetGroupsByUserIdAsync(user.UserId);

            var response = new UserResponse(
                user.UserId,
                user.Username,
                user.Email,
                roles.Select(r => r.RoleName),
                groups.Select(g => g.GroupName)
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex) {
            return Conflict(ex.Message);
        }
        catch (Exception ex) {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserByIdAsync(int userId) {
        try {
            var user = await userService.GetUserByIdAsync(userId);
            var roles = await userService.GetRolesByUserIdAsync(user.UserId);
            var groups = await userService.GetGroupsByUserIdAsync(user.UserId);
            var response = new UserResponse(
                user.UserId,
                user.Username,
                user.Email,
                roles.Select(r => r.RoleName),
                groups.Select(g => g.GroupName)
            );
            return Ok(response);
        }
        catch (InvalidOperationException) {
            return NotFound("User not found.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync() {
        var users = await userService.GetAllUsersAsync();
        var userResponses = new List<UserResponse>();

        foreach (var user in users) {
            var roles = await userService.GetRolesByUserIdAsync(user.UserId);
            var groups = await userService.GetGroupsByUserIdAsync(user.UserId);

            var response = new UserResponse(
                user.UserId,
                user.Username,
                user.Email,
                roles.Select(r => r.RoleName),
                groups.Select(g => g.GroupName)
            );

            userResponses.Add(response);
        }

        return Ok(userResponses);
    }


    [HttpGet("{userId}/groups")]
    public async Task<IActionResult> GetGroupsByUserIdAsync(int userId) {
        try {
            var groups = await userService.GetGroupsByUserIdAsync(userId);
            return Ok(groups);
        }
        catch (ArgumentException ex) {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{userId}/roles")]
    public async Task<IActionResult> GetRolesByUserIdAsync(int userId) {
        try {
            var roles = await userService.GetRolesByUserIdAsync(userId);
            return Ok(roles);
        }
        catch (ArgumentException ex) {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserAsync(int userId, [FromBody] UpdateUserRequest request) {
        try {
            var updatedUser = await userService.UpdateUserAsync(userId, request.UserName, request.Email);

            var roles = await userService.GetRolesByUserIdAsync(updatedUser.UserId);
            var groups = await userService.GetGroupsByUserIdAsync(updatedUser.UserId);

            var result = new UserResponse(
                updatedUser.UserId,
                updatedUser.Username,
                updatedUser.Email,
                roles.Select(r => r.RoleName),
                groups.Select(g => g.GroupName)
            );

            return Ok(result);
        }
        catch (ArgumentException ex) {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) {
            return NotFound(ex.Message);
        }
        catch (Exception ex) {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserAsync(int userId) {
        try {
            await userService.DeleteUserAsync(userId);
            return NoContent();
        }
        catch (InvalidOperationException) {
            return NotFound("User not found.");
        }
    }

    [HttpPut("{userId}/password")]
    public async Task<IActionResult> UpdatePasswordAsync(int userId, [FromBody] UpdatePasswordRequest request) {
        if (request == null)
            return BadRequest("Request body cannot be null.");

        try {
            await userService.UpdatePasswordAsync(userId, request.NewPassword);
            return NoContent();
        }
        catch (InvalidOperationException) {
            return NotFound("User not found.");
        }
    }

    [HttpPost("validate-password")]
    public async Task<IActionResult> ValidatePasswordAsync([FromBody] ValidatePasswordRequest request) {
        if (request == null)
            return BadRequest("Request body cannot be null.");

        try {
            var isValid = await userService.ValidatePasswordAsync(request.Username, request.Password);
            return Ok(isValid);
        }
        catch (InvalidOperationException) {
            return NotFound("User not found.");
        }
    }
    
    [HttpPost("{userId}/roles")]
    public async Task<IActionResult> AddRolesToUserAsync(int userId, [FromBody] List<int> roleIds)
    {
        if (roleIds == null || !roleIds.Any())
            return BadRequest("Role IDs cannot be null or empty.");

        try
        {
            await userService.AddRolesToUserAsync(userId, roleIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPost("{userId}/groups")]
    public async Task<IActionResult> AddGroupsToUserAsync(int userId, [FromBody] List<int> groupIds)
    {
        if (groupIds == null || !groupIds.Any())
            return BadRequest("Group IDs cannot be null or empty.");

        try
        {
            await userService.AddGroupsToUserAsync(userId, groupIds);
            return NoContent(); 
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpDelete("{userId}/roles")]
    public async Task<IActionResult> RemoveRolesFromUserAsync(int userId, [FromBody] List<int> roleIds)
    {
        if (roleIds == null || !roleIds.Any())
        {
            return BadRequest("Role IDs cannot be null or empty.");
        }

        try
        {
            await userService.RemoveRolesFromUserAsync(userId, roleIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpDelete("{userId}/groups")]
    public async Task<IActionResult> RemoveGroupsFromUserAsync(int userId, [FromBody] List<int> groupIds)
    {
        if (groupIds == null || !groupIds.Any())
        {
            return BadRequest("Group IDs cannot be null or empty.");
        }

        try
        {
            await userService.RemoveGroupsFromUserAsync(userId, groupIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

}
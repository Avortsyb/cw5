using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Responses;
using UserManagement.Application.Interfaces;
using UserManagement.Data.Entities;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllGroupsAsync()
    {
        try
        {
            var groups = await groupService.GetAllGroupsAsync();
            return Ok(groups);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{groupId:int}"), ActionName("GetGroupById")]
    public async Task<IActionResult> GetGroupByIdAsync(int groupId)
    {
        try
        {
            var group = await groupService.GetGroupByIdAsync(groupId);
            return Ok(group);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{groupId}/users")]
    public async Task<IActionResult> GetUsersByGroupIdAsync(int groupId)
    {
        try
        {
            var users = await groupService.GetUsersByGroupIdAsync(groupId);
            var responses = users.Select(u => new UserResponse(
                u.UserId,
                u.Username,
                u.Email
            )).ToList();
            return Ok(responses);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddGroupAsync([FromBody] string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return BadRequest("Group cannot be null.");

        try
        {
            var group = new Group { GroupName = groupName };
            await groupService.AddGroupAsync(group);
            return CreatedAtAction("GetGroupById", new { groupId = group.GroupId }, group);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{groupId}")]
    public async Task<IActionResult> UpdateGroupAsync(int groupId, [FromBody] string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return BadRequest("Group name cannot be empty.");

        try
        {
            await groupService.UpdateGroupAsync(groupId, groupName);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{groupId}")]
    public async Task<IActionResult> DeleteGroupAsync(int groupId)
    {
        try
        {
            await groupService.DeleteGroupAsync(groupId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Responses;
using UserManagement.Application.Interfaces;
using UserManagement.Application.Services;
using UserManagement.Data.Entities;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        try
        {
            var roles = await roleService.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{roleId:int}"), ActionName("GetRoleById")]
    public async Task<IActionResult> GetRoleByIdAsync(int roleId)
    {
        try
        {
            var role = await roleService.GetRoleByIdAsync(roleId);
            return Ok(role);
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

    [HttpGet("{roleId}/users")]
    public async Task<IActionResult> GetUsersByRoleIdAsync(int roleId)
    {
        try
        {
            var users = await roleService.GetUsersByRoleIdAsync(roleId);
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
    public async Task<IActionResult> AddRoleAsync([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name cannot be empty.");

        try
        {
            var role = new Role { RoleName = roleName };
            await roleService.AddRoleAsync(role);
            return CreatedAtAction("GetAllRoles", new { roleId = role.RoleId }, role);
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

    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateRoleAsync(int roleId, [FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name cannot be empty.");

        try
        {
            await roleService.UpdateRoleAsync(roleId, roleName);
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

    [HttpDelete("{roleId}")]
    public async Task<IActionResult> DeleteRoleAsync(int roleId)
    {
        try
        {
            await roleService.DeleteRoleAsync(roleId);
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
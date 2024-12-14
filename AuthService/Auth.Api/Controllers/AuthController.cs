using Microsoft.AspNetCore.Mvc;
using Auth.Application.Services;
using MassTransit;
using Microsoft.IdentityModel.Tokens;
using Shared.Contracts;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokenService, IRequestClient<AuthRequest> requestClient) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        try
        {
            var tokenResponse = await tokenService.LoginAsync(request);
            return Ok(tokenResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
        
    [HttpPost("validate-access-token")]
    public IActionResult ValidateAccessToken([FromBody] string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token cannot be null or empty.");
        }

        try
        {
            var claimsPrincipal = tokenService.ValidateAccessToken(token);
                
            return Ok(new
            {
                IsValid = true,
                Claims = claimsPrincipal?.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized("Token is expired.");
        }
        catch (SecurityTokenException)
        {
            return Unauthorized("Invalid token.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occurred while validating token: {ex.Message}");
        }
    }
      
    [HttpPost("validate-refresh-token")]
    public async Task<IActionResult> ValidateRefreshToken([FromBody] string refreshToken)
    {
        var isValid = await tokenService.ValidateRefreshTokenAsync(refreshToken);

        if (!isValid)
            return Unauthorized(new { Error = "Invalid or expired refresh token." });

        return Ok(new { IsValid = true });
    }
        
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] string refreshToken)
    {
        await tokenService.RevokeRefreshTokenAsync(refreshToken);
        return Ok(new { Message = "Refresh token revoked successfully." });
    }
}
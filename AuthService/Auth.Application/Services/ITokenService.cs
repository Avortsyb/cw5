using System.Security.Claims;
using Auth.Application.Models;
using Shared.Contracts;

namespace Auth.Application.Services;

public interface ITokenService
{
    string GenerateAccessTokenAsync(UserDto user);
    Task<string> GenerateRefreshTokenAsync(int userId);
    string GenerateNewRefreshToken();
    Task<bool> ValidateRefreshTokenAsync(string token);
    ClaimsPrincipal ValidateAccessToken(string token);
    Task RevokeRefreshTokenAsync(string token);
    Task<TokenResponse> LoginAsync(AuthRequest authRequest);
}
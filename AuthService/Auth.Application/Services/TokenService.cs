using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Application.Models;
using Microsoft.IdentityModel.Tokens;
using Auth.Data.Models;
using Auth.Data.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Contracts;
using Shared.Contracts.Utils;

namespace Auth.Application.Services;

public class TokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IRequestClient<AuthRequest> requestClient,
    IOptions<JwtSettings> jwtSettings,
    ILogger<TokenService> logger) : ITokenService {
    public string GenerateAccessTokenAsync(UserDto user) {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        if (user == null) {
            logger.LogWarning("Attempted to generate access token for a null user.");
            throw new InvalidOperationException("User not found.");
        }

        logger.LogInformation("Generating access token for user with ID {UserId}", user.Id);

        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        if (user.Roles.Any()) {
            logger.LogInformation("User {UserId} has roles: {Roles}", user.Id, string.Join(", ", user.Roles));
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        if (user.Groups.Any()) {
            logger.LogInformation("User {UserId} is in groups: {Groups}", user.Id, string.Join(", ", user.Groups));
            claims.AddRange(user.Groups.Select(group => new Claim("groups", group)));
        }
        
        var token = new JwtSecurityToken(
            jwtSettings.Value.Issuer,
            jwtSettings.Value.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        logger.LogInformation("Access token generated successfully for user with ID {UserId}", user.Id);

        return accessToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(int userId) {
        try {
            logger.LogInformation("Generating refresh token for user with ID {UserId}", userId);

            var refreshToken = new RefreshToken {
                Token = GenerateNewRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(30),
                UserId = userId,
                IsRevoked = false
            };

            await refreshTokenRepository.CreateAsync(refreshToken);

            logger.LogInformation("Refresh token generated and saved for user with ID {UserId}", userId);
            return refreshToken.Token;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error generating refresh token for user with ID {UserId}", userId);
            throw;
        }
    }
    public string GenerateNewRefreshToken() {
        var token = Guid.NewGuid().ToString();
        logger.LogDebug("New refresh token generated: {Token}", token);
        return token;
    }
    
    public async Task<bool> ValidateRefreshTokenAsync(string token) {
        try {
            logger.LogInformation("Validating refresh token: {Token}", token);
            var refreshToken = await refreshTokenRepository.GetByTokenAsync(token);
            var isValid = refreshToken != null && !refreshToken.IsRevoked && refreshToken.Expires > DateTime.UtcNow;

            if (!isValid) {
                logger.LogWarning("Refresh token validation failed for token: {Token}", token);
            }

            return isValid;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error validating refresh token: {Token}", token);
            throw;
        }
    }
    public ClaimsPrincipal ValidateAccessToken(string token) {
        try {
            logger.LogInformation("Validating access token.");
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, GetValidationParameters(), out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken && jwtToken.ValidTo < DateTime.UtcNow) {
                logger.LogWarning("Access token is expired.");
                throw new SecurityTokenExpiredException("Token is expired");
            }

            logger.LogInformation("Access token validation succeeded.");
            return principal;
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error validating access token.");
            throw new SecurityTokenException("Invalid token", ex);
        }
    }

    private TokenValidationParameters GetValidationParameters() {
        logger.LogDebug("Generating token validation parameters.");
        return new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Value.Issuer,
            ValidAudience = jwtSettings.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    }

    public async Task RevokeRefreshTokenAsync(string token) {
        try {
            logger.LogInformation("Revoking refresh token: {Token}", token);
            var refreshToken = await refreshTokenRepository.GetByTokenAsync(token);

            if (refreshToken != null) {
                refreshToken.IsRevoked = true;
                await refreshTokenRepository.UpdateAsync(refreshToken);
                logger.LogInformation("Refresh token revoked successfully: {Token}", token);
            } else {
                logger.LogWarning("No refresh token found for revocation: {Token}", token);
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error revoking refresh token: {Token}", token);
            throw;
        }
    }
    
    public async Task<TokenResponse> LoginAsync(AuthRequest authRequest) {
        try {
            logger.LogInformation("Processing login request for user: {Username}", authRequest.Username);

            var response = await requestClient.GetResponse<AuthResponse>(authRequest);

            if (!response.Message.Success) {
                logger.LogWarning("Login failed for user: {Username}. Reason: {Error}", authRequest.Username, response.Message.Error);
                throw new UnauthorizedAccessException(response.Message.Error);
            }

            var user = response.Message.User;
            var accessToken = GenerateAccessTokenAsync(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);

            logger.LogInformation("Login succeeded for user: {Username}", user.Username);
            return new TokenResponse {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user
            };
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error processing login for user: {Username}", authRequest.Username);
            throw;
        }
    }
}
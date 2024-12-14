using Auth.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Where(t => t.Token == token && !t.IsRevoked && t.Expires > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int refreshTokenId)
    {
        var refreshToken = await context.RefreshTokens.FindAsync(refreshTokenId);
        if (refreshToken != null)
        {
            context.RefreshTokens.Remove(refreshToken);
            await context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<RefreshToken>> GetAllTokensForUserAsync(int userId)
    {
        return await context.RefreshTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }
}
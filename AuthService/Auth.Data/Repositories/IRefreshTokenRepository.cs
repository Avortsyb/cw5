using Auth.Data.Models;

namespace Auth.Data.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task CreateAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(int refreshTokenId);
    Task<IEnumerable<RefreshToken>> GetAllTokensForUserAsync(int userId);

}
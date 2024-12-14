using UserManagement.Application.Interfaces;

namespace UserManagement.Application.Services;

public class PasswordService : IPasswordService
{
    public async Task<string> HashPasswordAsync(string password)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
    }

    public async Task<bool> ValidatePasswordAsync(string enteredPassword, string storedHash)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash));
    }
}
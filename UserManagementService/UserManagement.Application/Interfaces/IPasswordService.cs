namespace UserManagement.Application.Interfaces;

public interface IPasswordService
{
    Task<string> HashPasswordAsync(string password);
    Task<bool> ValidatePasswordAsync(string enteredPassword, string storedHash);
}
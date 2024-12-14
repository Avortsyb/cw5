namespace UserManagement.Api.Requests;

public record UpdateUserRequest(
    string? UserName,
    string? Email);
namespace UserManagement.Api.Responses;

public record UserResponse(
    int Id, 
    string UserName, 
    string Email,
    IEnumerable<string>? Roles = null,
    IEnumerable<string>? Groups = null
);

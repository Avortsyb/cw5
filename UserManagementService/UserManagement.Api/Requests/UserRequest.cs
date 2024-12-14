namespace UserManagement.Api.Requests;

public record UserRequest(
    string UserName,
    string Email,
    string Password,
    List<int> RoleIds,
    List<int>? GroupIds = null);
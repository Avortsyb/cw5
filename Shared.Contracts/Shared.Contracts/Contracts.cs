namespace Shared.Contracts
{
    public class AuthRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Groups { get; set; }
    }
    
    public class CheckGroupRequest
    {
        public int GroupId { get; set; }
    }

    public class CheckGroupResponse
    {
        public bool Exists { get; set; }
        public string GroupName { get; set; }
        public string Error { get; set; }
    }

    public class GroupDeletedEvent
    {
        public int GroupId { get; set; }
    }
}

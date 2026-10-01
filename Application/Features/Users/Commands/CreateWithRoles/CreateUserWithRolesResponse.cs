namespace Application.Features.Users.Commands.CreateWithRoles;

public class CreateUserWithRolesResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Status { get; set; }
    public List<int> RoleIds { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

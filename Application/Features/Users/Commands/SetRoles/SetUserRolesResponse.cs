namespace Application.Features.Users.Commands.SetRoles;

public class SetUserRolesResponse
{
    public int UserId { get; set; }
    public List<int> AssignedRoleIds { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

namespace Application.Features.Users.Queries.GetListWithRoles;

public class GetListUserWithRolesListItemDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public bool Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<UserRoleItemDto> Roles { get; set; } = new();
    public int? StajBasvuruId { get; set; }
    public string? Okul { get; set; }
    public string? Bolum { get; set; }
    public string? StajKonusu { get; set; }
}

public class UserRoleItemDto
{
    public int UserOperationClaimId { get; set; }
    public int OperationClaimId { get; set; }
    public string OperationClaimName { get; set; } = string.Empty;
}

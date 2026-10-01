using Core.Security.JWT;

namespace Application.Features.Auth.Commands.Login;

public class LoggedResponse
{
    public AccessToken AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
}
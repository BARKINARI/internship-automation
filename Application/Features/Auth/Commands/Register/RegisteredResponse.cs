using Core.Security.JWT;

namespace Application.Features.Auth.Commands.Register;

public class RegisteredResponse
{
    public AccessToken AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = string.Empty;
}
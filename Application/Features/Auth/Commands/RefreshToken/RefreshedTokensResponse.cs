using Core.Security.JWT;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshedTokensResponse
{
    public AccessToken AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = string.Empty;
}
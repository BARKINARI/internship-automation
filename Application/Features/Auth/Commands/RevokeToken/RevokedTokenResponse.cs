namespace Application.Features.Auth.Commands.RevokeToken;

public class RevokedTokenResponse
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
}
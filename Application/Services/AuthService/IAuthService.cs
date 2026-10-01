using Core.Security.Entities;
using Core.Security.JWT;

namespace Application.Services.AuthService;

public interface IAuthService
{
    Task<AccessToken> CreateAccessToken(User<int> user);
    Task<RefreshToken<int, int>> CreateRefreshToken(User<int> user, string ipAddress);
    Task<RefreshToken<int, int>> AddRefreshToken(RefreshToken<int, int> refreshToken);
    Task DeleteOldRefreshTokens(int userId);
    Task<RefreshToken<int, int>?> GetRefreshTokenByToken(string token);
    Task RevokeRefreshToken(RefreshToken<int, int> refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null);
    Task<RefreshToken<int, int>> RotateRefreshToken(User<int> user, RefreshToken<int, int> refreshToken, string ipAddress);
    Task RevokeDescendantRefreshTokens(RefreshToken<int, int> refreshToken, string ipAddress, string reason);
}
using Application.Services.Repositories;
using Core.Security.Entities;
using Core.Security.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper<int, int, int> _tokenHelper;
    private readonly TokenOptions _tokenOptions;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;

    public AuthManager(
        IUserOperationClaimRepository userOperationClaimRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenHelper<int, int, int> tokenHelper,
        IConfiguration configuration
    )
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>() 
            ?? throw new InvalidOperationException("TokenOptions not found in configuration.");
    }

    public async Task<AccessToken> CreateAccessToken(User<int> user)
    {
        IList<OperationClaim<int>> operationClaims = await _userOperationClaimRepository
            .GetOperationClaimsByUserIdAsync(user.Id);

        AccessToken accessToken = _tokenHelper.CreateToken(user, operationClaims);
        return accessToken;
    }

    public async Task<RefreshToken<int, int>> AddRefreshToken(RefreshToken<int, int> refreshToken)
    {
        RefreshToken<int, int> addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);
        return addedRefreshToken;
    }

    public async Task DeleteOldRefreshTokens(int userId)
    {
        List<RefreshToken<int, int>> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(userId, _tokenOptions.RefreshTokenTTL);
        if (refreshTokens.Count > 0)
            await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<RefreshToken<int, int>?> GetRefreshTokenByToken(string token)
    {
        return await _refreshTokenRepository.GetAsync(r => r.Token == token);
    }

    public async Task RevokeRefreshToken(RefreshToken<int, int> refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        refreshToken.RevokedDate = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }

    public async Task<RefreshToken<int, int>> RotateRefreshToken(User<int> user, RefreshToken<int, int> refreshToken, string ipAddress)
    {
        RefreshToken<int, int> newRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        await RevokeRefreshToken(refreshToken, ipAddress, reason: "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    public async Task RevokeDescendantRefreshTokens(RefreshToken<int, int> refreshToken, string ipAddress, string reason)
    {
        if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;

        RefreshToken<int, int>? childToken = await _refreshTokenRepository.GetAsync(r => r.Token == refreshToken.ReplacedByToken);

        if (childToken != null && childToken.RevokedDate != null && childToken.ExpirationDate <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else if (childToken != null)
            await RevokeDescendantRefreshTokens(childToken, ipAddress, reason);
    }

    public Task<RefreshToken<int, int>> CreateRefreshToken(User<int> user, string ipAddress)
    {
        RefreshToken<int, int> refreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        return Task.FromResult(refreshToken);
    }
}
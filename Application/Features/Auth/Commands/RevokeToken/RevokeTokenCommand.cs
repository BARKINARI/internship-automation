using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Core.Security.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommand : IRequest<RevokedTokenResponse>
{
    public string Token { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, RevokedTokenResponse>
    {
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;

        public RevokeTokenCommandHandler(IAuthService authService, AuthBusinessRules authBusinessRules)
        {
            _authService = authService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<RevokedTokenResponse> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            RefreshToken<int, int>? refreshToken = await _authService.GetRefreshTokenByToken(request.Token);
            _authBusinessRules.RefreshTokenShouldExist(refreshToken);
            _authBusinessRules.RefreshTokenShouldBeActive(refreshToken!);

            await _authService.RevokeRefreshToken(refreshToken!, request.IpAddress, reason: "Revoked without replacement");

            return new RevokedTokenResponse
            {
                Id = refreshToken!.Id,
                Token = refreshToken.Token
            };
        }
    }
}
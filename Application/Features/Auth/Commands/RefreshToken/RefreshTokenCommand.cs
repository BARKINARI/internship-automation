using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserService;
using Core.Security.Entities;
using Core.Security.JWT;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshedTokensResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshedTokensResponse>
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly AuthBusinessRules _authBusinessRules;

        public RefreshTokenCommandHandler(
            IAuthService authService,
            IUserService userService,
            AuthBusinessRules authBusinessRules
        )
        {
            _authService = authService;
            _userService = userService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<RefreshedTokensResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            RefreshToken<int, int>? refreshToken = await _authService.GetRefreshTokenByToken(request.RefreshToken);
            _authBusinessRules.RefreshTokenShouldExist(refreshToken);
            _authBusinessRules.RefreshTokenShouldBeActive(refreshToken!);

            User<int>? user = await _userService.GetById(refreshToken!.UserId);
            _authBusinessRules.UserShouldExistWhenSelected(user);

            RefreshToken<int, int> newRefreshToken = await _authService.RotateRefreshToken(user!, refreshToken, request.IpAddress);
            AccessToken createdAccessToken = await _authService.CreateAccessToken(user!);
            await _authService.DeleteOldRefreshTokens(user!.Id);

            return new RefreshedTokensResponse
            {
                AccessToken = createdAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
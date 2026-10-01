using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserService;
using Core.Application.Dtos;
using Core.Application.Pipelines.Logging;
using Core.Security.Entities;
using Core.Security.JWT;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<LoggedResponse>, ILoggableRequest
{
    public UserForLoginDto UserForLoginDto { get; set; } = default!;
    public string IpAddress { get; set; } = string.Empty;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoggedResponse>
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;

        public LoginCommandHandler(
            IUserService userService,
            IAuthService authService,
            AuthBusinessRules authBusinessRules
        )
        {
            _userService = userService;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<LoggedResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User<int>? user = await _userService.GetByEmail(request.UserForLoginDto.Email);
            _authBusinessRules.UserShouldExistWhenSelected(user);
            _authBusinessRules.UserPasswordShouldMatch(user!, request.UserForLoginDto.Password);

            AccessToken accessToken = await _authService.CreateAccessToken(user!);
            RefreshToken<int, int> refreshToken = await _authService.CreateRefreshToken(user!, request.IpAddress);
            await _authService.AddRefreshToken(refreshToken);
            await _authService.DeleteOldRefreshTokens(user!.Id);

            return new LoggedResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                FirstName = user!.FirstName,
                LastName = user!.LastName
            };
        }
    }
}
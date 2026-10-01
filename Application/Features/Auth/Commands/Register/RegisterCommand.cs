using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Core.Application.Dtos;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Entities;
using Core.Security.Hashing;
using Core.Security.JWT;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisteredResponse>, ITransactionalRequest, ILoggableRequest
{
    public UserForRegisterDto UserForRegisterDto { get; set; } = default!;
    public string IpAddress { get; set; } = string.Empty;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisteredResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly AuthBusinessRules _authBusinessRules;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IAuthService authService,
            AuthBusinessRules authBusinessRules
        )
        {
            _userRepository = userRepository;
            _authService = authService;
            _authBusinessRules = authBusinessRules;
        }

        public async Task<RegisteredResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await _authBusinessRules.EmailShouldNotExistsWhenRegister(request.UserForRegisterDto.Email);

            HashingHelper.CreatePasswordHash(
                request.UserForRegisterDto.Password,
                out byte[] passwordHash,
                out byte[] passwordSalt
            );

            User<int> newUser = new()
            {
                FirstName = request.UserForRegisterDto.FirstName,
                LastName = request.UserForRegisterDto.LastName,
                Email = request.UserForRegisterDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };

            User<int> createdUser = await _userRepository.AddAsync(newUser, cancellationToken);
            AccessToken createdAccessToken = await _authService.CreateAccessToken(createdUser);
            RefreshToken<int, int> createdRefreshToken = await _authService.CreateRefreshToken(createdUser, request.IpAddress);
            await _authService.AddRefreshToken(createdRefreshToken);

            return new RegisteredResponse
            {
                AccessToken = createdAccessToken,
                RefreshToken = createdRefreshToken.Token
            };
        }
    }
}
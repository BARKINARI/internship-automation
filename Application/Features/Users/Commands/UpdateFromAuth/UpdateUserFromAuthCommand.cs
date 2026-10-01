using Application.Features.Users.Rules;
using Application.Services.AuthService;
using Application.Services.Repositories;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Entities;
using Core.Security.Hashing;
using Core.Security.JWT;
using MediatR;

namespace Application.Features.Users.Commands.UpdateFromAuth;

public class UpdateUserFromAuthCommand : IRequest<UpdatedUserFromAuthResponse>, ITransactionalRequest, ILoggableRequest
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? OldPassword { get; set; }

    public class UpdateUserFromAuthCommandHandler : IRequestHandler<UpdateUserFromAuthCommand, UpdatedUserFromAuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserFromAuthCommandHandler(
            IUserRepository userRepository,
            IAuthService authService,
            UserBusinessRules userBusinessRules
        )
        {
            _userRepository = userRepository;
            _authService = authService;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdatedUserFromAuthResponse> Handle(UpdateUserFromAuthCommand request, CancellationToken cancellationToken)
        {
            User<int>? user = await _userRepository.GetAsync(predicate: u => u.Id == request.Id, cancellationToken: cancellationToken);
            _userBusinessRules.UserShouldExistWhenSelected(user);
            await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(request.Id, request.Email);

            if (!string.IsNullOrEmpty(request.OldPassword) && !string.IsNullOrEmpty(request.Password))
            {
                _userBusinessRules.UserPasswordShouldMatchToOldPassword(user!, request.OldPassword);
                HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user!.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            user!.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            User<int> updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
            AccessToken accessToken = await _authService.CreateAccessToken(updatedUser);

            return new UpdatedUserFromAuthResponse
            {
                Id = updatedUser.Id,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Email = updatedUser.Email,
                AccessToken = accessToken
            };
        }
    }
}
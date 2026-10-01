using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Entities;
using Core.Security.Hashing;
using MediatR;

namespace Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<UpdatedUserResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool Status { get; set; } = true;

    public string[] Roles => [UsersOperationClaims.Admin, UsersOperationClaims.Write, UsersOperationClaims.Update];

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdatedUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper, UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdatedUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            User<int>? user = await _userRepository.GetAsync(predicate: u => u.Id == request.Id, cancellationToken: cancellationToken);
            _userBusinessRules.UserShouldExistWhenSelected(user);
            await _userBusinessRules.UserEmailShouldNotExistsWhenUpdate(request.Id, request.Email);

            user!.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Status = request.Status;

            if (!string.IsNullOrEmpty(request.Password))
            {
                HashingHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            User<int> updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
            UpdatedUserResponse response = _mapper.Map<UpdatedUserResponse>(updatedUser);
            return response;
        }
    }
}
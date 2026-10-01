using Application.Features.Users.Rules;
using Application.Services.Repositories;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Constants;
using Core.Security.Entities;
using Core.Security.Hashing;
using MediatR;

namespace Application.Features.Users.Commands.CreateWithRoles;

public class CreateUserWithRolesCommand : IRequest<CreateUserWithRolesResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<int> OperationClaimIds { get; set; } = new();

    public string[] Roles => [GeneralOperationClaims.Admin, "Users.Write", "Users.Add"];

    public class CreateUserWithRolesCommandHandler : IRequestHandler<CreateUserWithRolesCommand, CreateUserWithRolesResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly UserBusinessRules _userBusinessRules;

        public CreateUserWithRolesCommandHandler(
            IUserRepository userRepository,
            IUserOperationClaimRepository userOperationClaimRepository,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userOperationClaimRepository = userOperationClaimRepository;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<CreateUserWithRolesResponse> Handle(CreateUserWithRolesCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserEmailShouldNotExistsWhenInsert(request.Email);

            string passwordToHash = string.IsNullOrWhiteSpace(request.Password) ? "Staj2026*" : request.Password.Trim();
            HashingHelper.CreatePasswordHash(passwordToHash, out byte[] passwordHash, out byte[] passwordSalt);

            User<int> user = new()
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true,
                CreatedDate = DateTime.UtcNow
            };

            User<int> createdUser = await _userRepository.AddAsync(user, cancellationToken);

            var assignedIds = new List<int>();
            if (request.OperationClaimIds != null && request.OperationClaimIds.Any())
            {
                foreach (var claimId in request.OperationClaimIds.Distinct())
                {
                    UserOperationClaim<int, int, int> claim = new(
                        userId: createdUser.Id,
                        operationClaimId: claimId
                    )
                    {
                        CreatedDate = DateTime.UtcNow
                    };
                    await _userOperationClaimRepository.AddAsync(claim, cancellationToken);
                    assignedIds.Add(claimId);
                }
            }

            return new CreateUserWithRolesResponse
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email,
                Status = createdUser.Status,
                RoleIds = assignedIds,
                Success = true,
                Message = $"'{createdUser.FirstName} {createdUser.LastName}' kullanıcısı başarıyla oluşturuldu ve yetkilendirildi."
            };
        }
    }
}

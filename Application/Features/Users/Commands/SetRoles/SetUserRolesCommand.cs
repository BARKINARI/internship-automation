using Application.Services.Repositories;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Constants;
using Core.Security.Entities;
using MediatR;

namespace Application.Features.Users.Commands.SetRoles;

public class SetUserRolesCommand : IRequest<SetUserRolesResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int UserId { get; set; }
    public List<int> OperationClaimIds { get; set; } = new();

    public string[] Roles => [GeneralOperationClaims.Admin, "Users.Write", "UserOperationClaims.Write"];

    public class SetUserRolesCommandHandler : IRequestHandler<SetUserRolesCommand, SetUserRolesResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;

        public SetUserRolesCommandHandler(
            IUserRepository userRepository,
            IUserOperationClaimRepository userOperationClaimRepository)
        {
            _userRepository = userRepository;
            _userOperationClaimRepository = userOperationClaimRepository;
        }

        public async Task<SetUserRolesResponse> Handle(SetUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
            if (user == null)
                throw new BusinessException("Kullanıcı bulunamadı.");

            var currentClaimsPaginate = await _userOperationClaimRepository.GetListAsync(
                predicate: uoc => uoc.UserId == request.UserId,
                size: 1000,
                cancellationToken: cancellationToken
            );

            var currentList = currentClaimsPaginate.Items.ToList();
            var targetIds = (request.OperationClaimIds ?? new List<int>()).Distinct().ToList();

            var toDelete = currentList.Where(c => !targetIds.Contains(c.OperationClaimId)).ToList();
            if (toDelete.Count > 0)
            {
                await _userOperationClaimRepository.DeleteRangeAsync(toDelete);
            }

            var existingClaimIds = currentList.Select(c => c.OperationClaimId).ToHashSet();
            var toAddIds = targetIds.Where(id => !existingClaimIds.Contains(id)).ToList();

            foreach (var claimId in toAddIds)
            {
                UserOperationClaim<int, int, int> newClaim = new(
                    userId: request.UserId,
                    operationClaimId: claimId
                )
                {
                    CreatedDate = DateTime.UtcNow
                };
                await _userOperationClaimRepository.AddAsync(newClaim, cancellationToken);
            }

            return new SetUserRolesResponse
            {
                UserId = request.UserId,
                AssignedRoleIds = targetIds,
                Success = true,
                Message = $"Kullanıcının ({user.FirstName} {user.LastName}) yetkileri başarıyla güncellendi."
            };
        }
    }
}

using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Entities;
using MediatR;

namespace Application.Features.UserOperationClaims.Commands.Update;

public class UpdateUserOperationClaimCommand : IRequest<UpdatedUserOperationClaimResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OperationClaimId { get; set; }

    public string[] Roles => [UserOperationClaimsOperationClaims.Admin, UserOperationClaimsOperationClaims.Write, UserOperationClaimsOperationClaims.Update];

    public class UpdateUserOperationClaimCommandHandler : IRequestHandler<UpdateUserOperationClaimCommand, UpdatedUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

        public UpdateUserOperationClaimCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules
        )
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<UpdatedUserOperationClaimResponse> Handle(UpdateUserOperationClaimCommand request, CancellationToken cancellationToken)
        {
            UserOperationClaim<int, int, int>? userOperationClaim = await _userOperationClaimRepository.GetAsync(
                predicate: u => u.Id == request.Id,
                cancellationToken: cancellationToken
            );
            _userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(userOperationClaim);
            await _userOperationClaimBusinessRules.UserShouldNotHaveDuplicateClaim(request.UserId, request.OperationClaimId);

            _mapper.Map(request, userOperationClaim);
            UserOperationClaim<int, int, int> updatedUserOperationClaim = await _userOperationClaimRepository.UpdateAsync(userOperationClaim!, cancellationToken);
            UpdatedUserOperationClaimResponse response = _mapper.Map<UpdatedUserOperationClaimResponse>(updatedUserOperationClaim);
            return response;
        }
    }
}
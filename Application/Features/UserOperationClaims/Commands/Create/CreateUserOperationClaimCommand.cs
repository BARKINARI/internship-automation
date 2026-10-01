using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Entities;
using MediatR;

namespace Application.Features.UserOperationClaims.Commands.Create;

public class CreateUserOperationClaimCommand : IRequest<CreatedUserOperationClaimResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int UserId { get; set; }
    public int OperationClaimId { get; set; }

    public string[] Roles => [UserOperationClaimsOperationClaims.Admin, UserOperationClaimsOperationClaims.Write, UserOperationClaimsOperationClaims.Add];

    public class CreateUserOperationClaimCommandHandler : IRequestHandler<CreateUserOperationClaimCommand, CreatedUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

        public CreateUserOperationClaimCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules
        )
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _mapper = mapper;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<CreatedUserOperationClaimResponse> Handle(CreateUserOperationClaimCommand request, CancellationToken cancellationToken)
        {
            await _userOperationClaimBusinessRules.UserShouldNotHaveDuplicateClaim(request.UserId, request.OperationClaimId);

            UserOperationClaim<int, int, int> userOperationClaim = _mapper.Map<UserOperationClaim<int, int, int>>(request);
            UserOperationClaim<int, int, int> createdUserOperationClaim = await _userOperationClaimRepository.AddAsync(userOperationClaim, cancellationToken);
            CreatedUserOperationClaimResponse response = _mapper.Map<CreatedUserOperationClaimResponse>(createdUserOperationClaim);
            return response;
        }
    }
}
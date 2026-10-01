using Application.Features.UserOperationClaims.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Entities;

namespace Application.Features.UserOperationClaims.Rules;

public class UserOperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;

    public UserOperationClaimBusinessRules(IUserOperationClaimRepository userOperationClaimRepository)
    {
        _userOperationClaimRepository = userOperationClaimRepository;
    }

    public void UserOperationClaimShouldExistWhenSelected(UserOperationClaim<int, int, int>? userOperationClaim)
    {
        if (userOperationClaim == null)
            throw new BusinessException(UserOperationClaimsMessages.UserOperationClaimNotExists);
    }

    public async Task UserOperationClaimIdShouldExistWhenSelected(int id)
    {
        UserOperationClaim<int, int, int>? userOperationClaim = await _userOperationClaimRepository.GetAsync(
            predicate: u => u.Id == id,
            enableTracking: false
        );
        UserOperationClaimShouldExistWhenSelected(userOperationClaim);
    }

    public async Task UserShouldNotHaveDuplicateClaim(int userId, int operationClaimId)
    {
        bool exists = await _userOperationClaimRepository.AnyAsync(
            predicate: u => u.UserId == userId && u.OperationClaimId == operationClaimId
        );
        if (exists)
            throw new BusinessException(UserOperationClaimsMessages.UserOperationClaimAlreadyExists);
    }
}
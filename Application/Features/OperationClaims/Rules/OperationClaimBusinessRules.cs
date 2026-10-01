using Application.Features.OperationClaims.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Entities;

namespace Application.Features.OperationClaims.Rules;

public class OperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IOperationClaimRepository _operationClaimRepository;

    public OperationClaimBusinessRules(IOperationClaimRepository operationClaimRepository)
    {
        _operationClaimRepository = operationClaimRepository;
    }

    public void OperationClaimShouldExistWhenSelected(OperationClaim<int>? operationClaim)
    {
        if (operationClaim == null)
            throw new BusinessException(OperationClaimsMessages.OperationClaimNotExists);
    }

    public async Task OperationClaimIdShouldExistWhenSelected(int id)
    {
        OperationClaim<int>? operationClaim = await _operationClaimRepository.GetAsync(predicate: o => o.Id == id, enableTracking: false);
        OperationClaimShouldExistWhenSelected(operationClaim);
    }

    public async Task OperationClaimNameShouldNotExistsWhenInsert(string name)
    {
        bool exists = await _operationClaimRepository.AnyAsync(predicate: o => o.Name == name);
        if (exists)
            throw new BusinessException(OperationClaimsMessages.OperationClaimAlreadyExists);
    }

    public async Task OperationClaimNameShouldNotExistsWhenUpdate(int id, string name)
    {
        bool exists = await _operationClaimRepository.AnyAsync(predicate: o => o.Id != id && o.Name == name);
        if (exists)
            throw new BusinessException(OperationClaimsMessages.OperationClaimAlreadyExists);
    }
}
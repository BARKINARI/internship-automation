using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserOperationClaimRepository : EfRepositoryBase<UserOperationClaim<int, int, int>, int, BaseDbContext>, IUserOperationClaimRepository
{
    public UserOperationClaimRepository(BaseDbContext context) : base(context) { }

    public async Task<IList<OperationClaim<int>>> GetOperationClaimsByUserIdAsync(int userId)
    {
        var operationClaims = await Context.UserOperationClaims
            .AsNoTracking()
            .Where(uoc => uoc.UserId == userId)
            .Join(
                Context.OperationClaims.AsNoTracking(),
                uoc => uoc.OperationClaimId,
                oc => oc.Id,
                (uoc, oc) => oc
            )
            .ToListAsync();

        return operationClaims;
    }
}
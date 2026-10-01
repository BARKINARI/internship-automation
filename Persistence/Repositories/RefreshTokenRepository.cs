using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken<int, int>, int, BaseDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext context) : base(context) { }

    public async Task<List<RefreshToken<int, int>>> GetOldRefreshTokensAsync(int userId, int ttl)
    {
        List<RefreshToken<int, int>> tokens = await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == userId
                && r.RevokedDate == null
                && r.ExpirationDate >= DateTime.UtcNow
                && r.CreatedDate.AddDays(ttl) <= DateTime.UtcNow
            )
            .ToListAsync();

        return tokens;
    }
}
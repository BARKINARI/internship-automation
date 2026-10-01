using System.Linq.Expressions;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.StajyerProjeService;

public interface IStajyerProjeService
{
    Task<StajyerProje?> GetAsync(
        Expression<Func<StajyerProje, bool>> predicate,
        Func<IQueryable<StajyerProje>, IIncludableQueryable<StajyerProje, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<StajyerProje>?> GetListAsync(
        Expression<Func<StajyerProje, bool>>? predicate = null,
        Func<IQueryable<StajyerProje>, IOrderedQueryable<StajyerProje>>? orderBy = null,
        Func<IQueryable<StajyerProje>, IIncludableQueryable<StajyerProje, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<StajyerProje> AddAsync(StajyerProje stajyerProje);
    Task<StajyerProje> UpdateAsync(StajyerProje stajyerProje);
    Task<StajyerProje> DeleteAsync(StajyerProje stajyerProje, bool permanent = false);
    Task<List<StajyerProje>> GetByStajBasvuruIdAsync(int stajBasvuruId);
    Task<List<StajyerProje>> GetByProjeIdAsync(int projeId);
}
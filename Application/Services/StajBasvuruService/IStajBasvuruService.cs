using System.Linq.Expressions;
using Core.Persistence.Paging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.StajBasvuruService;

public interface IStajBasvuruService
{
    Task<StajBasvuru?> GetAsync(
        Expression<Func<StajBasvuru, bool>> predicate,
        Func<IQueryable<StajBasvuru>, IIncludableQueryable<StajBasvuru, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<StajBasvuru>?> GetListAsync(
        Expression<Func<StajBasvuru, bool>>? predicate = null,
        Func<IQueryable<StajBasvuru>, IOrderedQueryable<StajBasvuru>>? orderBy = null,
        Func<IQueryable<StajBasvuru>, IIncludableQueryable<StajBasvuru, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<StajBasvuru> AddAsync(StajBasvuru stajBasvuru);
    Task<StajBasvuru> UpdateAsync(StajBasvuru stajBasvuru);
    Task<StajBasvuru> DeleteAsync(StajBasvuru stajBasvuru, bool permanent = false);
    Task<StajBasvuru> ChangeStatusAsync(int stajBasvuruId, StajBasvuruDurumu status);
}
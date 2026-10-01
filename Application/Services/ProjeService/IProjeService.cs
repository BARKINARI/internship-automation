using System.Linq.Expressions;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.ProjeService;

public interface IProjeService
{
    Task<Proje?> GetAsync(
        Expression<Func<Proje, bool>> predicate,
        Func<IQueryable<Proje>, IIncludableQueryable<Proje, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<Proje>?> GetListAsync(
        Expression<Func<Proje, bool>>? predicate = null,
        Func<IQueryable<Proje>, IOrderedQueryable<Proje>>? orderBy = null,
        Func<IQueryable<Proje>, IIncludableQueryable<Proje, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Proje> AddAsync(Proje proje);
    Task<Proje> UpdateAsync(Proje proje);
    Task<Proje> DeleteAsync(Proje proje, bool permanent = false);
}
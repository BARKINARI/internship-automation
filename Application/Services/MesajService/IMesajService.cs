using System.Linq.Expressions;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.MesajService;

public interface IMesajService
{
    Task<Mesaj?> GetAsync(
        Expression<Func<Mesaj, bool>> predicate,
        Func<IQueryable<Mesaj>, IIncludableQueryable<Mesaj, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<IPaginate<Mesaj>?> GetListAsync(
        Expression<Func<Mesaj, bool>>? predicate = null,
        Func<IQueryable<Mesaj>, IOrderedQueryable<Mesaj>>? orderBy = null,
        Func<IQueryable<Mesaj>, IIncludableQueryable<Mesaj, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Mesaj> AddAsync(Mesaj mesaj);
    Task<Mesaj> UpdateAsync(Mesaj mesaj);
    Task<Mesaj> DeleteAsync(Mesaj mesaj, bool permanent = false);
    Task<List<Mesaj>> GetConversationAsync(int gonderenId, int aliciId);
}
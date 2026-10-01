using System.Linq.Expressions;
using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.MesajService;

public class MesajManager : IMesajService
{
    private readonly IMesajRepository _mesajRepository;

    public MesajManager(IMesajRepository mesajRepository)
    {
        _mesajRepository = mesajRepository;
    }

    public async Task<Mesaj?> GetAsync(
        Expression<Func<Mesaj, bool>> predicate,
        Func<IQueryable<Mesaj>, IIncludableQueryable<Mesaj, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _mesajRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
    }

    public async Task<IPaginate<Mesaj>?> GetListAsync(
        Expression<Func<Mesaj, bool>>? predicate = null,
        Func<IQueryable<Mesaj>, IOrderedQueryable<Mesaj>>? orderBy = null,
        Func<IQueryable<Mesaj>, IIncludableQueryable<Mesaj, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _mesajRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
    }

    public async Task<Mesaj> AddAsync(Mesaj mesaj)
    {
        return await _mesajRepository.AddAsync(mesaj);
    }

    public async Task<Mesaj> UpdateAsync(Mesaj mesaj)
    {
        return await _mesajRepository.UpdateAsync(mesaj);
    }

    public async Task<Mesaj> DeleteAsync(Mesaj mesaj, bool permanent = false)
    {
        return await _mesajRepository.DeleteAsync(mesaj, permanent);
    }

    public async Task<List<Mesaj>> GetConversationAsync(int gonderenId, int aliciId)
    {
        var paginate = await _mesajRepository.GetListAsync(
            predicate: m => (m.GonderenId == gonderenId && m.AliciId == aliciId)
                         || (m.GonderenId == aliciId && m.AliciId == gonderenId),
            orderBy: q => q.OrderBy(m => m.CreatedDate),
            size: 500,
            enableTracking: false
        );
        return paginate.Items.ToList();
    }
}
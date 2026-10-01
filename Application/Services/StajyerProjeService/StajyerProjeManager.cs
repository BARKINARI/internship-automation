using System.Linq.Expressions;
using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.StajyerProjeService;

public class StajyerProjeManager : IStajyerProjeService
{
    private readonly IStajyerProjeRepository _stajyerProjeRepository;

    public StajyerProjeManager(IStajyerProjeRepository stajyerProjeRepository)
    {
        _stajyerProjeRepository = stajyerProjeRepository;
    }

    public async Task<StajyerProje?> GetAsync(
        Expression<Func<StajyerProje, bool>> predicate,
        Func<IQueryable<StajyerProje>, IIncludableQueryable<StajyerProje, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _stajyerProjeRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
    }

    public async Task<IPaginate<StajyerProje>?> GetListAsync(
        Expression<Func<StajyerProje, bool>>? predicate = null,
        Func<IQueryable<StajyerProje>, IOrderedQueryable<StajyerProje>>? orderBy = null,
        Func<IQueryable<StajyerProje>, IIncludableQueryable<StajyerProje, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _stajyerProjeRepository.GetListAsync(
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

    public async Task<StajyerProje> AddAsync(StajyerProje stajyerProje)
    {
        return await _stajyerProjeRepository.AddAsync(stajyerProje);
    }

    public async Task<StajyerProje> UpdateAsync(StajyerProje stajyerProje)
    {
        return await _stajyerProjeRepository.UpdateAsync(stajyerProje);
    }

    public async Task<StajyerProje> DeleteAsync(StajyerProje stajyerProje, bool permanent = false)
    {
        return await _stajyerProjeRepository.DeleteAsync(stajyerProje, permanent);
    }

    public async Task<List<StajyerProje>> GetByStajBasvuruIdAsync(int stajBasvuruId)
    {
        var paginate = await _stajyerProjeRepository.GetListAsync(
            predicate: sp => sp.StajBasvuruId == stajBasvuruId,
            size: 100,
            enableTracking: false
        );
        return paginate.Items.ToList();
    }

    public async Task<List<StajyerProje>> GetByProjeIdAsync(int projeId)
    {
        var paginate = await _stajyerProjeRepository.GetListAsync(
            predicate: sp => sp.ProjeId == projeId,
            size: 100,
            enableTracking: false
        );
        return paginate.Items.ToList();
    }
}
using System.Linq.Expressions;
using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.ProjeService;

public class ProjeManager : IProjeService
{
    private readonly IProjeRepository _projeRepository;

    public ProjeManager(IProjeRepository projeRepository)
    {
        _projeRepository = projeRepository;
    }

    public async Task<Proje?> GetAsync(
        Expression<Func<Proje, bool>> predicate,
        Func<IQueryable<Proje>, IIncludableQueryable<Proje, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _projeRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
    }

    public async Task<IPaginate<Proje>?> GetListAsync(
        Expression<Func<Proje, bool>>? predicate = null,
        Func<IQueryable<Proje>, IOrderedQueryable<Proje>>? orderBy = null,
        Func<IQueryable<Proje>, IIncludableQueryable<Proje, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _projeRepository.GetListAsync(
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

    public async Task<Proje> AddAsync(Proje proje)
    {
        return await _projeRepository.AddAsync(proje);
    }

    public async Task<Proje> UpdateAsync(Proje proje)
    {
        return await _projeRepository.UpdateAsync(proje);
    }

    public async Task<Proje> DeleteAsync(Proje proje, bool permanent = false)
    {
        return await _projeRepository.DeleteAsync(proje, permanent);
    }
}
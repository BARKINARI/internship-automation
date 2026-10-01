using System.Linq.Expressions;
using Application.Features.StajBasvurulari.Rules;
using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.Services.StajBasvuruService;

public class StajBasvuruManager : IStajBasvuruService
{
    private readonly IStajBasvuruRepository _stajBasvuruRepository;
    private readonly StajBasvuruBusinessRules _stajBasvuruBusinessRules;

    public StajBasvuruManager(IStajBasvuruRepository stajBasvuruRepository, StajBasvuruBusinessRules stajBasvuruBusinessRules)
    {
        _stajBasvuruRepository = stajBasvuruRepository;
        _stajBasvuruBusinessRules = stajBasvuruBusinessRules;
    }

    public async Task<StajBasvuru?> GetAsync(
        Expression<Func<StajBasvuru, bool>> predicate,
        Func<IQueryable<StajBasvuru>, IIncludableQueryable<StajBasvuru, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        StajBasvuru? stajBasvuru = await _stajBasvuruRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return stajBasvuru;
    }

    public async Task<IPaginate<StajBasvuru>?> GetListAsync(
        Expression<Func<StajBasvuru, bool>>? predicate = null,
        Func<IQueryable<StajBasvuru>, IOrderedQueryable<StajBasvuru>>? orderBy = null,
        Func<IQueryable<StajBasvuru>, IIncludableQueryable<StajBasvuru, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<StajBasvuru> stajBasvuruList = await _stajBasvuruRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return stajBasvuruList;
    }

    public async Task<StajBasvuru> AddAsync(StajBasvuru stajBasvuru)
    {
        StajBasvuru addedStajBasvuru = await _stajBasvuruRepository.AddAsync(stajBasvuru);
        return addedStajBasvuru;
    }

    public async Task<StajBasvuru> UpdateAsync(StajBasvuru stajBasvuru)
    {
        StajBasvuru updatedStajBasvuru = await _stajBasvuruRepository.UpdateAsync(stajBasvuru);
        return updatedStajBasvuru;
    }

    public async Task<StajBasvuru> DeleteAsync(StajBasvuru stajBasvuru, bool permanent = false)
    {
        StajBasvuru deletedStajBasvuru = await _stajBasvuruRepository.DeleteAsync(stajBasvuru, permanent);
        return deletedStajBasvuru;
    }

    public async Task<StajBasvuru> ChangeStatusAsync(int stajBasvuruId, StajBasvuruDurumu status)
    {
        StajBasvuru? stajBasvuru = await _stajBasvuruRepository.GetAsync(predicate: s => s.Id == stajBasvuruId);
        _stajBasvuruBusinessRules.StajBasvuruShouldExistWhenSelected(stajBasvuru);

        stajBasvuru!.StajBasvuruDurumu = status;
        StajBasvuru updatedStajBasvuru = await _stajBasvuruRepository.UpdateAsync(stajBasvuru);
        return updatedStajBasvuru;
    }
}
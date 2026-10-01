using Application.Features.StajyerProjeler.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Domain.Entities;

namespace Application.Features.StajyerProjeler.Rules;

public class StajyerProjeBusinessRules : BaseBusinessRules
{
    private readonly IStajyerProjeRepository _stajyerProjeRepository;

    public StajyerProjeBusinessRules(IStajyerProjeRepository stajyerProjeRepository)
    {
        _stajyerProjeRepository = stajyerProjeRepository;
    }

    public void StajyerProjeShouldExistWhenSelected(StajyerProje? stajyerProje)
    {
        if (stajyerProje == null)
            throw new BusinessException(StajyerProjeMessages.StajyerProjeNotExists);
    }

    public async Task StajyerProjeIdShouldExistWhenSelected(int id)
    {
        StajyerProje? stajyerProje = await _stajyerProjeRepository.GetAsync(predicate: sp => sp.Id == id, enableTracking: false);
        StajyerProjeShouldExistWhenSelected(stajyerProje);
    }

    public async Task StajyerCannotBeAssignedToSameProjectTwice(int stajBasvuruId, int projeId)
    {
        bool exists = await _stajyerProjeRepository.AnyAsync(
            predicate: sp => sp.StajBasvuruId == stajBasvuruId && sp.ProjeId == projeId
        );
        if (exists)
            throw new BusinessException(StajyerProjeMessages.StajyerProjeAlreadyExists);
    }

    public Task PuanShouldBeValid(int? puan)
    {
        if (puan.HasValue && (puan.Value < 0 || puan.Value > 100))
            throw new BusinessException(StajyerProjeMessages.PuanGecersiz);
        return Task.CompletedTask;
    }
}
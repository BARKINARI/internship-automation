using Application.Features.Projeler.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Domain.Entities;

namespace Application.Features.Projeler.Rules;

public class ProjeBusinessRules : BaseBusinessRules
{
    private readonly IProjeRepository _projeRepository;

    public ProjeBusinessRules(IProjeRepository projeRepository)
    {
        _projeRepository = projeRepository;
    }

    public void ProjeShouldExistWhenSelected(Proje? proje)
    {
        if (proje == null)
            throw new BusinessException(ProjeMessages.ProjeNotExists);
    }

    public async Task ProjeIdShouldExistWhenSelected(int id)
    {
        Proje? proje = await _projeRepository.GetAsync(predicate: p => p.Id == id, enableTracking: false);
        ProjeShouldExistWhenSelected(proje);
    }
}

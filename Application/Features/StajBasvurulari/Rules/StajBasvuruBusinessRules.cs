using Application.Features.StajBasvurulari.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.StajBasvurulari.Rules;

public class StajBasvuruBusinessRules : BaseBusinessRules
{
    private readonly IStajBasvuruRepository _stajBasvuruRepository;

    public StajBasvuruBusinessRules(IStajBasvuruRepository stajBasvuruRepository)
    {
        _stajBasvuruRepository = stajBasvuruRepository;
    }

    public void StajBasvuruShouldExistWhenSelected(StajBasvuru? stajBasvuru)
    {
        if (stajBasvuru == null)
            throw new BusinessException(StajBasvuruMessages.StajBasvuruNotExists);
    }

    public async Task StajBasvuruIdShouldExistWhenSelected(int id)
    {
        StajBasvuru? stajBasvuru = await _stajBasvuruRepository.GetAsync(predicate: s => s.Id == id, enableTracking: false);
        StajBasvuruShouldExistWhenSelected(stajBasvuru);
    }

    public Task StajTarihleriGecerliOlmali(DateTime baslangic, DateTime bitis)
    {
        if (baslangic >= bitis)
            throw new BusinessException(StajBasvuruMessages.BaslangicTarihiBitisindenBuyukOlamaz);
        return Task.CompletedTask;
    }

    public async Task AyniEpostaIleAktifBasvuruOlamaz(string eposta)
    {
        bool exists = await _stajBasvuruRepository.AnyAsync(
            predicate: s => s.Eposta == eposta && s.StajBasvuruDurumu == StajBasvuruDurumu.Beklemede
        );
        if (exists)
            throw new BusinessException(StajBasvuruMessages.AyniEpostaIleAktifBasvuruVar);
    }
}
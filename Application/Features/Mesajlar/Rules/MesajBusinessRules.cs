using Application.Features.Mesajlar.Constants;
using Application.Services.Repositories;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exception.Types;
using Domain.Entities;

namespace Application.Features.Mesajlar.Rules;

public class MesajBusinessRules : BaseBusinessRules
{
    private readonly IMesajRepository _mesajRepository;

    public MesajBusinessRules(IMesajRepository mesajRepository)
    {
        _mesajRepository = mesajRepository;
    }

    public void MesajShouldExistWhenSelected(Mesaj? mesaj)
    {
        if (mesaj == null)
            throw new BusinessException(MesajMessages.MesajNotExists);
    }

    public void MesajContentShouldNotBeEmpty(string? icerik, string? dosyaYolu = null)
    {
        if (string.IsNullOrWhiteSpace(icerik) && string.IsNullOrWhiteSpace(dosyaYolu))
            throw new BusinessException(MesajMessages.MesajIcerikBosOlamaz);
    }
}

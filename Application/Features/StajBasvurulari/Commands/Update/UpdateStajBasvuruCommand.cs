using Application.Features.StajBasvurulari.Constants;
using Application.Features.StajBasvurulari.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.StajBasvurulari.Commands.Update;

public class UpdateStajBasvuruCommand : IRequest<UpdatedStajBasvuruResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest, ICacheRemoverRequest
{
    public int Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;
    public string Okul { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Sinif { get; set; } = string.Empty;
    public DateTime StajBaslangicTarihi { get; set; }
    public DateTime StajBitisTarihi { get; set; }
    public string? Referans { get; set; }
    public string? KendiniTanitmaYazisi { get; set; }
    public string? CvDosyaYolu { get; set; }
    public string? StajKonusu { get; set; }
    public StajBasvuruDurumu StajBasvuruDurumu { get; set; }

    public string[] Roles => [StajBasvuruOperationClaims.Admin, StajBasvuruOperationClaims.Write, StajBasvuruOperationClaims.Update, StajBasvuruOperationClaims.StajBasvurular, StajBasvuruOperationClaims.StajSorumlusu];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetStajBasvurulari"];

    public class UpdateStajBasvuruCommandHandler : IRequestHandler<UpdateStajBasvuruCommand, UpdatedStajBasvuruResponse>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IMapper _mapper;
        private readonly StajBasvuruBusinessRules _stajBasvuruBusinessRules;

        public UpdateStajBasvuruCommandHandler(
            IStajBasvuruRepository stajBasvuruRepository,
            IMapper mapper,
            StajBasvuruBusinessRules stajBasvuruBusinessRules
        )
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _mapper = mapper;
            _stajBasvuruBusinessRules = stajBasvuruBusinessRules;
        }

        public async Task<UpdatedStajBasvuruResponse> Handle(UpdateStajBasvuruCommand request, CancellationToken cancellationToken)
        {
            StajBasvuru? stajBasvuru = await _stajBasvuruRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            _stajBasvuruBusinessRules.StajBasvuruShouldExistWhenSelected(stajBasvuru);
            await _stajBasvuruBusinessRules.StajTarihleriGecerliOlmali(request.StajBaslangicTarihi, request.StajBitisTarihi);

            request.StajBaslangicTarihi = DateTime.SpecifyKind(request.StajBaslangicTarihi, DateTimeKind.Utc);
            request.StajBitisTarihi = DateTime.SpecifyKind(request.StajBitisTarihi, DateTimeKind.Utc);

            _mapper.Map(request, stajBasvuru);
            stajBasvuru!.StajBaslangicTarihi = DateTime.SpecifyKind(stajBasvuru.StajBaslangicTarihi, DateTimeKind.Utc);
            stajBasvuru.StajBitisTarihi = DateTime.SpecifyKind(stajBasvuru.StajBitisTarihi, DateTimeKind.Utc);
            StajBasvuru updatedStajBasvuru = await _stajBasvuruRepository.UpdateAsync(stajBasvuru, cancellationToken);
            UpdatedStajBasvuruResponse response = _mapper.Map<UpdatedStajBasvuruResponse>(updatedStajBasvuru);
            return response;
        }
    }
}
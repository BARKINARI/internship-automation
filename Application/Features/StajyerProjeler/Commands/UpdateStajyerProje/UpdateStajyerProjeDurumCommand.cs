using Application.Features.StajyerProjeler.Rules;
using Application.Services.Repositories;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.StajyerProjeler.Commands.UpdateStajyerProje;

public class UpdateStajyerProjeDurumCommand : IRequest<bool>, ITransactionalRequest, ILoggableRequest
{
    public int StajyerProjeId { get; set; }
    public StajyerProjeDurumu Durum { get; set; }
    public string? TeslimAciklamasi { get; set; }
    public string? DosyaYolu { get; set; }
    public int? Puan { get; set; }
    public string? YoneticiNotu { get; set; }

    public class UpdateStajyerProjeDurumCommandHandler : IRequestHandler<UpdateStajyerProjeDurumCommand, bool>
    {
        private readonly IStajyerProjeRepository _stajyerProjeRepository;
        private readonly StajyerProjeBusinessRules _stajyerProjeBusinessRules;

        public UpdateStajyerProjeDurumCommandHandler(
            IStajyerProjeRepository stajyerProjeRepository,
            StajyerProjeBusinessRules stajyerProjeBusinessRules
        )
        {
            _stajyerProjeRepository = stajyerProjeRepository;
            _stajyerProjeBusinessRules = stajyerProjeBusinessRules;
        }

        public async Task<bool> Handle(UpdateStajyerProjeDurumCommand request, CancellationToken cancellationToken)
        {
            StajyerProje? sp = await _stajyerProjeRepository.GetAsync(x => x.Id == request.StajyerProjeId, cancellationToken: cancellationToken);
            if (sp == null) return false;

            await _stajyerProjeBusinessRules.PuanShouldBeValid(request.Puan);

            sp.Durum = request.Durum;

            if (!string.IsNullOrWhiteSpace(request.TeslimAciklamasi))
                sp.TeslimAciklamasi = request.TeslimAciklamasi.Trim();

            if (!string.IsNullOrWhiteSpace(request.DosyaYolu))
                sp.DosyaYolu = request.DosyaYolu.Trim();

            if (request.Puan.HasValue)
                sp.Puan = request.Puan.Value;

            if (!string.IsNullOrWhiteSpace(request.YoneticiNotu))
                sp.YoneticiNotu = request.YoneticiNotu.Trim();

            if (request.Durum == StajyerProjeDurumu.TeslimEdildi && !sp.TeslimTarihi.HasValue)
                sp.TeslimTarihi = DateTime.UtcNow;

            sp.UpdatedDate = DateTime.UtcNow;

            await _stajyerProjeRepository.UpdateAsync(sp, cancellationToken);
            return true;
        }
    }
}
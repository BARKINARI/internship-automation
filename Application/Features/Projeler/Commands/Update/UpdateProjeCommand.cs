using Application.Features.Projeler.Rules;
using Application.Services.Repositories;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Projeler.Commands.Update;

public class UpdateProjeCommand : IRequest<UpdatedProjeResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public ProjeDurumu Durum { get; set; }
    public int? YoneticiId { get; set; }

    public string[] Roles => [GeneralOperationClaims.Admin, "Proje.Yonetimi", "ProjeYoneticisi"];

    public class UpdateProjeCommandHandler : IRequestHandler<UpdateProjeCommand, UpdatedProjeResponse>
    {
        private readonly IProjeRepository _projeRepository;
        private readonly ProjeBusinessRules _projeBusinessRules;

        public UpdateProjeCommandHandler(IProjeRepository projeRepository, ProjeBusinessRules projeBusinessRules)
        {
            _projeRepository = projeRepository;
            _projeBusinessRules = projeBusinessRules;
        }

        public async Task<UpdatedProjeResponse> Handle(UpdateProjeCommand request, CancellationToken cancellationToken)
        {
            Proje? proje = await _projeRepository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
            _projeBusinessRules.ProjeShouldExistWhenSelected(proje);

            proje!.Baslik = request.Baslik.Trim();
            proje.Aciklama = request.Aciklama.Trim();
            proje.BaslangicTarihi = DateTime.SpecifyKind(request.BaslangicTarihi, DateTimeKind.Utc);
            proje.BitisTarihi = DateTime.SpecifyKind(request.BitisTarihi, DateTimeKind.Utc);
            proje.Durum = request.Durum;
            if (request.YoneticiId.HasValue) proje.YoneticiId = request.YoneticiId;
            proje.UpdatedDate = DateTime.UtcNow;

            await _projeRepository.UpdateAsync(proje, cancellationToken);

            return new UpdatedProjeResponse
            {
                Id = proje.Id,
                Baslik = proje.Baslik,
                Aciklama = proje.Aciklama,
                BaslangicTarihi = proje.BaslangicTarihi,
                BitisTarihi = proje.BitisTarihi,
                Durum = proje.Durum,
                YoneticiId = proje.YoneticiId,
                UpdatedDate = proje.UpdatedDate.Value
            };
        }
    }
}

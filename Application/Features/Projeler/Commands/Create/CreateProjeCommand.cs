using Application.Services.Repositories;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Projeler.Commands.Create;

public class CreateProjeCommand : IRequest<CreatedProjeResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public ProjeDurumu Durum { get; set; } = ProjeDurumu.Beklemede;
    public int? YoneticiId { get; set; }

    public string[] Roles => [GeneralOperationClaims.Admin, "Proje.Yonetimi", "ProjeYoneticisi"];

    public class CreateProjeCommandHandler : IRequestHandler<CreateProjeCommand, CreatedProjeResponse>
    {
        private readonly IProjeRepository _projeRepository;

        public CreateProjeCommandHandler(IProjeRepository projeRepository)
        {
            _projeRepository = projeRepository;
        }

        public async Task<CreatedProjeResponse> Handle(CreateProjeCommand request, CancellationToken cancellationToken)
        {
            Proje proje = new()
            {
                Baslik = request.Baslik.Trim(),
                Aciklama = request.Aciklama.Trim(),
                BaslangicTarihi = DateTime.SpecifyKind(request.BaslangicTarihi, DateTimeKind.Utc),
                BitisTarihi = DateTime.SpecifyKind(request.BitisTarihi, DateTimeKind.Utc),
                Durum = request.Durum,
                YoneticiId = request.YoneticiId,
                CreatedDate = DateTime.UtcNow
            };

            Proje created = await _projeRepository.AddAsync(proje, cancellationToken);

            return new CreatedProjeResponse
            {
                Id = created.Id,
                Baslik = created.Baslik,
                Aciklama = created.Aciklama,
                BaslangicTarihi = created.BaslangicTarihi,
                BitisTarihi = created.BitisTarihi,
                Durum = created.Durum,
                YoneticiId = created.YoneticiId,
                CreatedDate = created.CreatedDate
            };
        }
    }
}

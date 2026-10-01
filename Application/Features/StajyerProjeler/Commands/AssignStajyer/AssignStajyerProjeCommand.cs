using Application.Services.MesajService;
using Application.Services.ProjeService;
using Application.Services.Repositories;
using Application.Services.StajBasvuruService;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.StajyerProjeler.Commands.AssignStajyer;

public class AssignStajyerProjeCommand : IRequest<AssignedStajyerProjeResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int ProjeId { get; set; }
    public int StajBasvuruId { get; set; }

    public string[] Roles => [GeneralOperationClaims.Admin, "Proje.TakipNotlandirma", "Proje.Yonetimi", "ProjeYoneticisi"];

    public class AssignStajyerProjeCommandHandler : IRequestHandler<AssignStajyerProjeCommand, AssignedStajyerProjeResponse>
    {
        private readonly IProjeService _projeService;
        private readonly IStajyerProjeRepository _stajyerProjeRepository;
        private readonly IStajBasvuruService _stajBasvuruService;
        private readonly IMesajService _mesajService;

        public AssignStajyerProjeCommandHandler(
            IProjeService projeService,
            IStajyerProjeRepository stajyerProjeRepository,
            IStajBasvuruService stajBasvuruService,
            IMesajService mesajService
        )
        {
            _projeService = projeService;
            _stajyerProjeRepository = stajyerProjeRepository;
            _stajBasvuruService = stajBasvuruService;
            _mesajService = mesajService;
        }

        public async Task<AssignedStajyerProjeResponse> Handle(AssignStajyerProjeCommand request, CancellationToken cancellationToken)
        {
            Proje? proje = await _projeService.GetAsync(p => p.Id == request.ProjeId, cancellationToken: cancellationToken);
            if (proje == null)
            {
                return new AssignedStajyerProjeResponse { Success = false, Message = "Proje bulunamadı." };
            }

            StajBasvuru? basvuru = await _stajBasvuruService.GetAsync(s => s.Id == request.StajBasvuruId, cancellationToken: cancellationToken);
            if (basvuru == null)
            {
                return new AssignedStajyerProjeResponse { Success = false, Message = "Staj başvurusu bulunamadı." };
            }

            StajyerProje? existing = await _stajyerProjeRepository.GetAsync(
                sp => sp.ProjeId == request.ProjeId && sp.StajBasvuruId == request.StajBasvuruId,
                cancellationToken: cancellationToken
            );

            if (existing != null)
            {
                return new AssignedStajyerProjeResponse
                {
                    Success = false,
                    Message = "Bu proje ilgili stajyere zaten atanmış durumda."
                };
            }

            StajyerProje stajyerProje = new()
            {
                ProjeId = request.ProjeId,
                StajBasvuruId = request.StajBasvuruId,
                UserId = basvuru.UserId,
                AtanmaTarihi = DateTime.UtcNow,
                Durum = StajyerProjeDurumu.Atandi,
                CreatedDate = DateTime.UtcNow
            };

            StajyerProje created = await _stajyerProjeRepository.AddAsync(stajyerProje, cancellationToken);

            if (basvuru.UserId.HasValue)
            {
                int adminId = proje.YoneticiId ?? 1;
                Mesaj notifyMsg = new()
                {
                    GonderenId = adminId,
                    AliciId = basvuru.UserId.Value,
                    StajBasvuruId = basvuru.Id,
                    ProjeId = proje.Id,
                    Icerik = $"📌 Yeni bir proje görevi atandı: '{proje.Baslik}'. Proje detaylarını ve teslim tarihini 'Projelerim' alanından inceleyebilirsiniz.",
                    OkunduMu = false,
                    CreatedDate = DateTime.UtcNow
                };
                await _mesajService.AddAsync(notifyMsg);
            }

            return new AssignedStajyerProjeResponse
            {
                Id = created.Id,
                ProjeId = proje.Id,
                ProjeBaslik = proje.Baslik,
                StajBasvuruId = basvuru.Id,
                StajyerAdSoyad = $"{basvuru.Ad} {basvuru.Soyad}",
                AtanmaTarihi = created.AtanmaTarihi,
                Durum = created.Durum,
                Success = true,
                Message = $"'{proje.Baslik}' projesi '{basvuru.Ad} {basvuru.Soyad}' isimli stajyere başarıyla atandı."
            };
        }
    }
}
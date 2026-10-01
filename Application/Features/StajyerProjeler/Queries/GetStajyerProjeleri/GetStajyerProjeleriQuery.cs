using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StajyerProjeler.Queries.GetStajyerProjeleri;

public class GetStajyerProjeleriQuery : IRequest<List<GetStajyerProjeleriListItemDto>>
{
    public int? StajBasvuruId { get; set; }
    public int? UserId { get; set; }
    public int? ProjeId { get; set; }

    public class GetStajyerProjeleriQueryHandler : IRequestHandler<GetStajyerProjeleriQuery, List<GetStajyerProjeleriListItemDto>>
    {
        private readonly IStajyerProjeRepository _stajyerProjeRepository;

        public GetStajyerProjeleriQueryHandler(IStajyerProjeRepository stajyerProjeRepository)
        {
            _stajyerProjeRepository = stajyerProjeRepository;
        }

        public async Task<List<GetStajyerProjeleriListItemDto>> Handle(GetStajyerProjeleriQuery request, CancellationToken cancellationToken)
        {
            IPaginate<StajyerProje> paginate = await _stajyerProjeRepository.GetListAsync(
                predicate: sp =>
                    (!request.StajBasvuruId.HasValue || sp.StajBasvuruId == request.StajBasvuruId.Value) &&
                    (!request.UserId.HasValue || sp.UserId == request.UserId.Value) &&
                    (!request.ProjeId.HasValue || sp.ProjeId == request.ProjeId.Value),
                include: q => q.Include(sp => sp.Proje)
                               .Include(sp => sp.StajBasvuru),
                orderBy: q => q.OrderByDescending(sp => sp.AtanmaTarihi),
                size: 500,
                cancellationToken: cancellationToken
            );

            return paginate.Items.Select(sp => new GetStajyerProjeleriListItemDto
            {
                StajyerProjeId = sp.Id,
                ProjeId = sp.ProjeId,
                ProjeBaslik = sp.Proje != null ? sp.Proje.Baslik : "Proje",
                ProjeAciklama = sp.Proje != null ? sp.Proje.Aciklama : "",
                BaslangicTarihi = sp.Proje != null ? sp.Proje.BaslangicTarihi : sp.AtanmaTarihi,
                BitisTarihi = sp.Proje != null ? sp.Proje.BitisTarihi : sp.AtanmaTarihi,
                StajBasvuruId = sp.StajBasvuruId,
                StajyerAdSoyad = sp.StajBasvuru != null ? $"{sp.StajBasvuru.Ad} {sp.StajBasvuru.Soyad}" : "Stajyer",
                StajyerEposta = sp.StajBasvuru != null ? sp.StajBasvuru.Eposta : "",
                AtanmaTarihi = sp.AtanmaTarihi,
                TeslimTarihi = sp.TeslimTarihi,
                Durum = sp.Durum,
                TeslimAciklamasi = sp.TeslimAciklamasi,
                DosyaYolu = sp.DosyaYolu,
                Puan = sp.Puan,
                YoneticiNotu = sp.YoneticiNotu
            }).ToList();
        }
    }
}
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projeler.Queries.GetById;

public class GetByIdProjeQuery : IRequest<GetByIdProjeResponse?>
{
    public int Id { get; set; }

    public class GetByIdProjeQueryHandler : IRequestHandler<GetByIdProjeQuery, GetByIdProjeResponse?>
    {
        private readonly IProjeRepository _projeRepository;

        public GetByIdProjeQueryHandler(IProjeRepository projeRepository)
        {
            _projeRepository = projeRepository;
        }

        public async Task<GetByIdProjeResponse?> Handle(GetByIdProjeQuery request, CancellationToken cancellationToken)
        {
            Proje? p = await _projeRepository.GetAsync(
                predicate: x => x.Id == request.Id,
                include: q => q.Include(x => x.Yonetici)
                               .Include(x => x.StajyerProjeler)
                                   .ThenInclude(sp => sp.StajBasvuru),
                cancellationToken: cancellationToken
            );

            if (p == null) return null;

            return new GetByIdProjeResponse
            {
                Id = p.Id,
                Baslik = p.Baslik,
                Aciklama = p.Aciklama,
                BaslangicTarihi = p.BaslangicTarihi,
                BitisTarihi = p.BitisTarihi,
                Durum = p.Durum,
                YoneticiId = p.YoneticiId,
                YoneticiAd = p.Yonetici != null ? $"{p.Yonetici.FirstName} {p.Yonetici.LastName}" : "Yönetici",
                CreatedDate = p.CreatedDate,
                Stajyerler = p.StajyerProjeler.Select(sp => new ProjeStajyerItemDto
                {
                    StajyerProjeId = sp.Id,
                    StajBasvuruId = sp.StajBasvuruId,
                    StajyerAdSoyad = sp.StajBasvuru != null ? $"{sp.StajBasvuru.Ad} {sp.StajBasvuru.Soyad}" : "Stajyer",
                    Eposta = sp.StajBasvuru != null ? sp.StajBasvuru.Eposta : "",
                    AtanmaTarihi = sp.AtanmaTarihi,
                    TeslimTarihi = sp.TeslimTarihi,
                    Durum = sp.Durum,
                    TeslimAciklamasi = sp.TeslimAciklamasi,
                    DosyaYolu = sp.DosyaYolu,
                    Puan = sp.Puan,
                    YoneticiNotu = sp.YoneticiNotu
                }).ToList()
            };
        }
    }
}

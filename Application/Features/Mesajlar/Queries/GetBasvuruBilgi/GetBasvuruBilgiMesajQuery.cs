using Application.Services.StajBasvuruService;
using Domain.Entities;
using MediatR;

namespace Application.Features.Mesajlar.Queries.GetBasvuruBilgi;

public class GetBasvuruBilgiMesajQuery : IRequest<OgrenciBasvuruBilgiDto?>
{
    public int StajBasvuruId { get; set; }

    public class GetBasvuruBilgiMesajQueryHandler : IRequestHandler<GetBasvuruBilgiMesajQuery, OgrenciBasvuruBilgiDto?>
    {
        private readonly IStajBasvuruService _stajBasvuruService;

        public GetBasvuruBilgiMesajQueryHandler(IStajBasvuruService stajBasvuruService)
        {
            _stajBasvuruService = stajBasvuruService;
        }

        public async Task<OgrenciBasvuruBilgiDto?> Handle(GetBasvuruBilgiMesajQuery request, CancellationToken cancellationToken)
        {
            StajBasvuru? s = await _stajBasvuruService.GetAsync(
                predicate: x => x.Id == request.StajBasvuruId,
                cancellationToken: cancellationToken
            );

            if (s == null) return null;

            return new OgrenciBasvuruBilgiDto
            {
                Id = s.Id,
                UserId = s.UserId,
                AdSoyad = $"{s.Ad} {s.Soyad}",
                Eposta = s.Eposta,
                Telefon = s.Telefon,
                Okul = s.Okul,
                Bolum = s.Bolum,
                Sinif = s.Sinif,
                StajKonusu = s.StajKonusu,
                StajBaslangicTarihi = s.StajBaslangicTarihi,
                StajBitisTarihi = s.StajBitisTarihi,
                StajBasvuruDurumu = (int)s.StajBasvuruDurumu
            };
        }
    }
}

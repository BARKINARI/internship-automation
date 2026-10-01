using Application.Features.Mesajlar.Queries.GetBasvuruBilgi;
using Application.Services.StajBasvuruService;
using Domain.Entities;
using MediatR;

namespace Application.Features.Mesajlar.Queries.GetBasvuruByEmail;

public class GetBasvuruByEmailMesajQuery : IRequest<OgrenciBasvuruBilgiDto?>
{
    public string Email { get; set; } = string.Empty;

    public class GetBasvuruByEmailMesajQueryHandler : IRequestHandler<GetBasvuruByEmailMesajQuery, OgrenciBasvuruBilgiDto?>
    {
        private readonly IStajBasvuruService _stajBasvuruService;

        public GetBasvuruByEmailMesajQueryHandler(IStajBasvuruService stajBasvuruService)
        {
            _stajBasvuruService = stajBasvuruService;
        }

        public async Task<OgrenciBasvuruBilgiDto?> Handle(GetBasvuruByEmailMesajQuery request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            StajBasvuru? s = await _stajBasvuruService.GetAsync(
                predicate: x => x.Eposta.ToLower() == normalizedEmail && !x.DeletedDate.HasValue,
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

using Application.Services.StajBasvuruService;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Mesajlar.Queries.OgrenciGiris;

public class OgrenciGirisQuery : IRequest<OgrenciGirisResponse>
{
    public string Eposta { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;

    public class OgrenciGirisQueryHandler : IRequestHandler<OgrenciGirisQuery, OgrenciGirisResponse>
    {
        private readonly IStajBasvuruService _stajBasvuruService;

        public OgrenciGirisQueryHandler(IStajBasvuruService stajBasvuruService)
        {
            _stajBasvuruService = stajBasvuruService;
        }

        public async Task<OgrenciGirisResponse> Handle(OgrenciGirisQuery request, CancellationToken cancellationToken)
        {
            string cleanEmail = (request.Eposta ?? string.Empty).Trim().ToLower();
            string cleanPhone = (request.Telefon ?? string.Empty).Trim().Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

            var basvurular = await _stajBasvuruService.GetListAsync(
                predicate: s => s.Eposta.ToLower() == cleanEmail,
                cancellationToken: cancellationToken
            );

            var matched = basvurular?.Items.FirstOrDefault(s =>
            {
                string sPhone = (s.Telefon ?? string.Empty).Trim().Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                return sPhone == cleanPhone || sPhone.EndsWith(cleanPhone) || cleanPhone.EndsWith(sPhone);
            });

            if (matched == null)
            {
                return new OgrenciGirisResponse
                {
                    Basarili = false,
                    Mesaj = "Girilen e-posta ve telefon bilgileri ile eşleşen bir staj başvurusu bulunamadı. Lütfen bilgilerinizi kontrol ediniz."
                };
            }

            if (matched.StajBasvuruDurumu == StajBasvuruDurumu.Beklemede)
            {
                return new OgrenciGirisResponse
                {
                    Basarili = false,
                    StajBasvuruDurumu = (int)matched.StajBasvuruDurumu,
                    Mesaj = "Staj başvurunuz henüz değerlendirme aşamasındadır. Başvurunuz onaylandıktan sonra bu panelden staj koordinatörünüz ile iletişime geçebilirsiniz."
                };
            }

            if (matched.StajBasvuruDurumu == StajBasvuruDurumu.Reddedildi)
            {
                return new OgrenciGirisResponse
                {
                    Basarili = false,
                    StajBasvuruDurumu = (int)matched.StajBasvuruDurumu,
                    Mesaj = "Staj başvurunuz maalesef kabul edilmemiştir."
                };
            }

            if (matched.StajBasvuruDurumu == StajBasvuruDurumu.KabulEdildi)
            {
                return new OgrenciGirisResponse
                {
                    Basarili = true,
                    StajBasvuruId = matched.Id,
                    OgrenciAdSoyad = $"{matched.Ad} {matched.Soyad}",
                    Okul = matched.Okul,
                    Bolum = matched.Bolum,
                    StajKonusu = matched.StajKonusu,
                    StajBasvuruDurumu = (int)matched.StajBasvuruDurumu,
                    Mesaj = "Giriş başarılı."
                };
            }

            return new OgrenciGirisResponse
            {
                Basarili = false,
                Mesaj = "Başvuru durumu belirlenemedi."
            };
        }
    }
}

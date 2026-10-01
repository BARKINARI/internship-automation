using Application.Features.StajBasvurulari.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Mailing;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using MimeKit;

namespace Application.Features.StajBasvurulari.Commands.Create;

public class CreateStajBasvuruCommand : IRequest<CreatedStajBasvuruResponse>, ITransactionalRequest, ILoggableRequest, ICacheRemoverRequest
{
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
    public StajBasvuruDurumu StajBasvuruDurumu { get; set; } = StajBasvuruDurumu.Beklemede;

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetStajBasvurulari"];

    public class CreateStajBasvuruCommandHandler : IRequestHandler<CreateStajBasvuruCommand, CreatedStajBasvuruResponse>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IMapper _mapper;
        private readonly StajBasvuruBusinessRules _stajBasvuruBusinessRules;
        private readonly IMailService _mailService;

        public CreateStajBasvuruCommandHandler(
            IStajBasvuruRepository stajBasvuruRepository,
            IMapper mapper,
            StajBasvuruBusinessRules stajBasvuruBusinessRules,
            IMailService mailService
        )
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _mapper = mapper;
            _stajBasvuruBusinessRules = stajBasvuruBusinessRules;
            _mailService = mailService;
        }

        public async Task<CreatedStajBasvuruResponse> Handle(CreateStajBasvuruCommand request, CancellationToken cancellationToken)
        {
            request.StajBaslangicTarihi = DateTime.SpecifyKind(request.StajBaslangicTarihi, DateTimeKind.Utc);
            request.StajBitisTarihi = DateTime.SpecifyKind(request.StajBitisTarihi, DateTimeKind.Utc);

            await _stajBasvuruBusinessRules.StajTarihleriGecerliOlmali(request.StajBaslangicTarihi, request.StajBitisTarihi);
            await _stajBasvuruBusinessRules.AyniEpostaIleAktifBasvuruOlamaz(request.Eposta);

            StajBasvuru mappedStajBasvuru = _mapper.Map<StajBasvuru>(request);
            mappedStajBasvuru.StajBaslangicTarihi = DateTime.SpecifyKind(mappedStajBasvuru.StajBaslangicTarihi, DateTimeKind.Utc);
            mappedStajBasvuru.StajBitisTarihi = DateTime.SpecifyKind(mappedStajBasvuru.StajBitisTarihi, DateTimeKind.Utc);

            StajBasvuru createdStajBasvuru = await _stajBasvuruRepository.AddAsync(mappedStajBasvuru, cancellationToken);
            CreatedStajBasvuruResponse response = _mapper.Map<CreatedStajBasvuruResponse>(createdStajBasvuru);

            await SendConfirmationEmailAsync(createdStajBasvuru);

            return response;
        }

        private async Task SendConfirmationEmailAsync(StajBasvuru basvuru)
        {
            try
            {
                string subject = "Staj Başvurunuz Alınmıştır - AE Yazılım";

                string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <div style='background-color: #4e73df; padding: 15px; border-radius: 6px 6px 0 0; text-align: center; color: white;'>
                        <h2 style='margin: 0;'>AE Yazılım Staj Sistemi</h2>
                    </div>
                    <div style='padding: 20px 10px;'>
                        <p style='font-size: 16px; color: #333;'>Sayın <strong>{basvuru.Ad} {basvuru.Soyad}</strong>,</p>
                        <p style='color: #555; line-height: 1.6;'>
                            Staj başvurunuz sistemimize başarıyla ulaşmıştır. Başvurunuz ilgili birimlerimiz tarafından incelendikten sonra en kısa sürede tarafınıza geri dönüş yapılacaktır.
                        </p>
                        
                        <div style='background-color: #f8f9fc; padding: 15px; border-left: 4px solid #4e73df; margin: 20px 0;'>
                            <h4 style='margin-top: 0; color: #4e73df;'>Başvuru Özetiniz:</h4>
                            <p style='margin: 5px 0; color: #555;'><strong>Üniversite:</strong> {basvuru.Okul}</p>
                            <p style='margin: 5px 0; color: #555;'><strong>Bölüm:</strong> {basvuru.Bolum}</p>
                            <p style='margin: 5px 0; color: #555;'><strong>Sınıf:</strong> {basvuru.Sinif}</p>
                            <p style='margin: 5px 0; color: #555;'><strong>Staj Alanı:</strong> {basvuru.StajKonusu ?? "Genel Yazılım"}</p>
                            <p style='margin: 5px 0; color: #555;'><strong>Staj Tarihleri:</strong> {basvuru.StajBaslangicTarihi:dd.MM.yyyy} - {basvuru.StajBitisTarihi:dd.MM.yyyy}</p>
                        </div>

                        <p style='color: #777; font-size: 14px;'>Süreçle ilgili gelişmeleri bu e-posta adresi üzerinden takip edebilirsiniz.</p>
                        <p style='color: #333; margin-top: 25px;'>İyi çalışmalar ve başarılar dileriz.<br><strong>AE Yazılım</strong></p>
                    </div>
                </div>";

                List<MailboxAddress> toList = [new MailboxAddress($"{basvuru.Ad} {basvuru.Soyad}", basvuru.Eposta)];

                Mail mail = new()
                {
                    Subject = subject,
                    TextBody = $"Sayın {basvuru.Ad} {basvuru.Soyad}, staj başvurunuz başarıyla alınmıştır. En yakın sürede size geri dönüş yapılacaktır.",
                    HtmlBody = htmlBody,
                    ToList = toList
                };

                await _mailService.SendEmailAsync(mail);
            }
            catch
            {
            }
        }
    }
}
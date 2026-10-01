using Application.Services.Repositories;
using Application.Services.UserService;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Mailing;
using Core.Security.Constants;
using Core.Security.Entities;
using Core.Security.Hashing;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using MimeKit;

namespace Application.Features.StajBasvurulari.Commands.ApproveAndSendPassword;

public class ApproveAndSendPasswordCommand : IRequest<ApproveAndSendPasswordResponse>, ISecuredRequest, ICacheRemoverRequest
{
    public int StajBasvuruId { get; set; }
    public string Password { get; set; } = string.Empty;

    public string[] Roles => [GeneralOperationClaims.Admin, "Staj.Basvurular", "StajSorumlusu"];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetStajBasvurulari"];

    public class ApproveAndSendPasswordCommandHandler : IRequestHandler<ApproveAndSendPasswordCommand, ApproveAndSendPasswordResponse>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;

        public ApproveAndSendPasswordCommandHandler(
            IStajBasvuruRepository stajBasvuruRepository,
            IUserService userService,
            IMailService mailService
        )
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _userService = userService;
            _mailService = mailService;
        }

        public async Task<ApproveAndSendPasswordResponse> Handle(ApproveAndSendPasswordCommand request, CancellationToken cancellationToken)
        {
            StajBasvuru? basvuru = await _stajBasvuruRepository.GetAsync(
                predicate: s => s.Id == request.StajBasvuruId,
                cancellationToken: cancellationToken
            );

            if (basvuru == null)
            {
                return new ApproveAndSendPasswordResponse
                {
                    Success = false,
                    Message = "Staj başvurusu bulunamadı."
                };
            }

            basvuru.StajBasvuruDurumu = StajBasvuruDurumu.KabulEdildi;
            await _stajBasvuruRepository.UpdateAsync(basvuru, cancellationToken);

            string passwordToSet = string.IsNullOrWhiteSpace(request.Password) ? "Staj2026*" : request.Password.Trim();
            HashingHelper.CreatePasswordHash(passwordToSet, out byte[] passwordHash, out byte[] passwordSalt);

            var normalizedEmail = basvuru.Eposta.Trim();
            User<int>? user = await _userService.GetByEmail(normalizedEmail);

            if (user != null)
            {
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Status = true;
                await _userService.Update(user);
            }
            else
            {
                user = new User<int>
                {
                    FirstName = basvuru.Ad,
                    LastName = basvuru.Soyad,
                    Email = normalizedEmail,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true
                };
                user = await _userService.Add(user);
            }

            basvuru.UserId = user.Id;
            await _stajBasvuruRepository.UpdateAsync(basvuru, cancellationToken);

            await SendApprovalEmailAsync(basvuru, passwordToSet);

            return new ApproveAndSendPasswordResponse
            {
                Success = true,
                Message = $"Staj başvurusu onaylandı ve giriş şifresi '{basvuru.Eposta}' adresine başarıyla gönderildi.",
                Email = basvuru.Eposta,
                StajBasvuruId = basvuru.Id
            };
        }

        private async Task SendApprovalEmailAsync(StajBasvuru basvuru, string password)
        {
            try
            {
                string subject = "Tebrikler! Staj Başvurunuz Onaylandı - AE Yazılım";

                string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <div style='background-color: #1cc88a; padding: 15px; border-radius: 6px 6px 0 0; text-align: center; color: white;'>
                        <h2 style='margin: 0;'>AE Yazılım Staj Sistemi</h2>
                        <p style='margin: 5px 0 0 0; font-size: 14px;'>Staj Başvuru Onayı & Giriş Bilgileri</p>
                    </div>
                    <div style='padding: 20px 15px;'>
                        <p style='font-size: 16px; color: #333;'>Sayın <strong>{basvuru.Ad} {basvuru.Soyad}</strong>,</p>
                        <p style='color: #555; line-height: 1.6;'>
                            Tebrikler! <strong>AE Yazılım</strong> bünyesindeki staj başvurunuz başarıyla <strong>onaylanmıştır</strong>.
                        </p>
                        <p style='color: #555; line-height: 1.6;'>
                            Staj süreciniz boyunca yapacağınız projeler, görevler ve aklınıza takılan sorularınız için sistemimizdeki stajyer mesajlaşma platformunu kullanabilirsiniz.
                        </p>
                        
                        <div style='background-color: #f8f9fc; padding: 20px; border-left: 4px solid #1cc88a; border-radius: 4px; margin: 20px 0;'>
                            <h4 style='margin-top: 0; color: #1cc88a; font-size: 16px;'>Sisteme Giriş Bilgileriniz:</h4>
                            <p style='margin: 8px 0; color: #333;'><strong>Giriş Adresi:</strong> <a href='https://localhost:7295/Login/Index' style='color: #4e73df; font-weight: bold;'>Giriş Sayfasına Git (/Login/Index)</a></p>
                            <p style='margin: 8px 0; color: #333;'><strong>E-Posta:</strong> {basvuru.Eposta}</p>
                            <p style='margin: 8px 0; color: #333;'><strong>Giriş Şifreniz:</strong> <span style='font-family: monospace; background-color: #e2e8f0; padding: 4px 10px; border-radius: 4px; font-weight: bold; color: #e74a3b;'>{password}</span></p>
                        </div>

                        <div style='background-color: #fff3cd; color: #856404; padding: 12px 15px; border-radius: 4px; margin-bottom: 20px; font-size: 13px;'>
                            ℹ️ Giriş ekranında yukarıdaki e-posta ve şifrenizi kullanarak oturum açabilir, yöneticinizle doğrudan mesajlaşmaya başlayabilirsiniz.
                        </div>

                        <p style='color: #777; font-size: 14px;'>Aramıza hoş geldiniz, başarılı ve verimli bir staj dönemi dileriz.</p>
                        <p style='color: #333; margin-top: 25px;'>Saygılarımızla,<br><strong>AE Yazılım Ekibi</strong></p>
                    </div>
                </div>";

                List<MailboxAddress> toList = [new MailboxAddress($"{basvuru.Ad} {basvuru.Soyad}", basvuru.Eposta)];

                Mail mail = new()
                {
                    ToList = toList,
                    Subject = subject,
                    HtmlBody = htmlBody
                };

                await _mailService.SendEmailAsync(mail);
            }
            catch
            {
            }
        }
    }
}

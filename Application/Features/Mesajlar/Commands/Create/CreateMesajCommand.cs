using Application.Features.Mesajlar.Rules;
using Application.Services.Repositories;
using Application.Services.UserService;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Domain.Entities;
using MediatR;

namespace Application.Features.Mesajlar.Commands.Create;

public class CreateMesajCommand : IRequest<CreatedMesajResponse>, ITransactionalRequest, ILoggableRequest
{
    public int GonderenId { get; set; }
    public int AliciId { get; set; }
    public int? StajBasvuruId { get; set; }
    public int? ProjeId { get; set; }
    public string Icerik { get; set; } = string.Empty;
    public string? DosyaYolu { get; set; }
    public string? DosyaAdi { get; set; }

    public class CreateMesajCommandHandler : IRequestHandler<CreateMesajCommand, CreatedMesajResponse>
    {
        private readonly IMesajRepository _mesajRepository;
        private readonly IUserService _userService;
        private readonly MesajBusinessRules _mesajBusinessRules;

        public CreateMesajCommandHandler(
            IMesajRepository mesajRepository,
            IUserService userService,
            MesajBusinessRules mesajBusinessRules
        )
        {
            _mesajRepository = mesajRepository;
            _userService = userService;
            _mesajBusinessRules = mesajBusinessRules;
        }

        public async Task<CreatedMesajResponse> Handle(CreateMesajCommand request, CancellationToken cancellationToken)
        {
            _mesajBusinessRules.MesajContentShouldNotBeEmpty(request.Icerik, request.DosyaYolu);

            Mesaj mesaj = new()
            {
                GonderenId = request.GonderenId,
                AliciId = request.AliciId,
                StajBasvuruId = request.StajBasvuruId,
                ProjeId = request.ProjeId,
                Icerik = (request.Icerik ?? string.Empty).Trim(),
                DosyaYolu = request.DosyaYolu,
                DosyaAdi = request.DosyaAdi,
                OkunduMu = false,
                CreatedDate = DateTime.UtcNow
            };

            Mesaj createdMesaj = await _mesajRepository.AddAsync(mesaj, cancellationToken);

            var gonderen = await _userService.GetById(request.GonderenId);
            var alici = await _userService.GetById(request.AliciId);

            return new CreatedMesajResponse
            {
                Id = createdMesaj.Id,
                GonderenId = createdMesaj.GonderenId,
                GonderenAd = gonderen != null ? $"{gonderen.FirstName} {gonderen.LastName}" : "Kullanıcı",
                AliciId = createdMesaj.AliciId,
                AliciAd = alici != null ? $"{alici.FirstName} {alici.LastName}" : "Kullanıcı",
                StajBasvuruId = createdMesaj.StajBasvuruId,
                ProjeId = createdMesaj.ProjeId,
                Icerik = createdMesaj.Icerik,
                DosyaYolu = createdMesaj.DosyaYolu,
                DosyaAdi = createdMesaj.DosyaAdi,
                OkunduMu = createdMesaj.OkunduMu,
                CreatedDate = createdMesaj.CreatedDate
            };
        }
    }
}

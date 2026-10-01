using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Mesajlar.Queries.GetListByStajBasvuruId;

public class GetListByStajBasvuruIdMesajQuery : IRequest<List<GetListByStajBasvuruIdMesajListItemDto>>
{
    public int StajBasvuruId { get; set; }
    public int? ProjeId { get; set; }
    public int? OkuyanUserId { get; set; }
    public string? OkuyanRol { get; set; }

    public class GetListByStajBasvuruIdMesajQueryHandler : IRequestHandler<GetListByStajBasvuruIdMesajQuery, List<GetListByStajBasvuruIdMesajListItemDto>>
    {
        private readonly IMesajRepository _mesajRepository;

        public GetListByStajBasvuruIdMesajQueryHandler(IMesajRepository mesajRepository)
        {
            _mesajRepository = mesajRepository;
        }

        public async Task<List<GetListByStajBasvuruIdMesajListItemDto>> Handle(GetListByStajBasvuruIdMesajQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Mesaj> paginate = await _mesajRepository.GetListAsync(
                predicate: m => m.StajBasvuruId == request.StajBasvuruId &&
                                (!request.ProjeId.HasValue ? true : m.ProjeId == request.ProjeId.Value),
                include: q => q.Include(m => m.GonderenUser).Include(m => m.AliciUser),
                orderBy: q => q.OrderBy(m => m.CreatedDate),
                size: 1000,
                cancellationToken: cancellationToken
            );

            if (request.OkuyanUserId.HasValue)
            {
                var unreadMessages = paginate.Items
                    .Where(m => !m.OkunduMu && m.AliciId == request.OkuyanUserId.Value)
                    .ToList();

                if (unreadMessages.Count > 0)
                {
                    foreach (var msg in unreadMessages)
                        msg.OkunduMu = true;

                    await _mesajRepository.UpdateRangeAsync(unreadMessages);
                }
            }

            var response = paginate.Items.Select(m => new GetListByStajBasvuruIdMesajListItemDto
            {
                Id = m.Id,
                GonderenId = m.GonderenId,
                GonderenAd = m.GonderenUser != null ? $"{m.GonderenUser.FirstName} {m.GonderenUser.LastName}" : "Kullanıcı",
                AliciId = m.AliciId,
                AliciAd = m.AliciUser != null ? $"{m.AliciUser.FirstName} {m.AliciUser.LastName}" : "Kullanıcı",
                StajBasvuruId = m.StajBasvuruId,
                ProjeId = m.ProjeId,
                Icerik = m.Icerik,
                DosyaYolu = m.DosyaYolu,
                DosyaAdi = m.DosyaAdi,
                OkunduMu = m.OkunduMu,
                CreatedDate = m.CreatedDate
            }).ToList();

            return response;
        }
    }
}

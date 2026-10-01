using Application.Services.Repositories;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Projeler.Queries.GetList;

public class GetListProjeQuery : IRequest<List<GetListProjeListItemDto>>
{
    public class GetListProjeQueryHandler : IRequestHandler<GetListProjeQuery, List<GetListProjeListItemDto>>
    {
        private readonly IProjeRepository _projeRepository;

        public GetListProjeQueryHandler(IProjeRepository projeRepository)
        {
            _projeRepository = projeRepository;
        }

        public async Task<List<GetListProjeListItemDto>> Handle(GetListProjeQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Proje> paginate = await _projeRepository.GetListAsync(
                include: q => q.Include(p => p.Yonetici).Include(p => p.StajyerProjeler),
                orderBy: q => q.OrderByDescending(p => p.Id),
                size: 500,
                cancellationToken: cancellationToken
            );

            return paginate.Items.Select(p => new GetListProjeListItemDto
            {
                Id = p.Id,
                Baslik = p.Baslik,
                Aciklama = p.Aciklama,
                BaslangicTarihi = p.BaslangicTarihi,
                BitisTarihi = p.BitisTarihi,
                Durum = p.Durum,
                YoneticiId = p.YoneticiId,
                YoneticiAd = p.Yonetici != null ? $"{p.Yonetici.FirstName} {p.Yonetici.LastName}" : "Yönetici",
                AtananStajyerSayisi = p.StajyerProjeler.Count,
                CreatedDate = p.CreatedDate
            }).ToList();
        }
    }
}

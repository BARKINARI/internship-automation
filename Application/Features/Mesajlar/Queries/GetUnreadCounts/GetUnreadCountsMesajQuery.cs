using Application.Services.Repositories;
using MediatR;

namespace Application.Features.Mesajlar.Queries.GetUnreadCounts;

public class GetUnreadCountsMesajQuery : IRequest<Dictionary<int, int>>
{
    public int AliciId { get; set; }

    public class GetUnreadCountsMesajQueryHandler : IRequestHandler<GetUnreadCountsMesajQuery, Dictionary<int, int>>
    {
        private readonly IMesajRepository _mesajRepository;

        public GetUnreadCountsMesajQueryHandler(IMesajRepository mesajRepository)
        {
            _mesajRepository = mesajRepository;
        }

        public async Task<Dictionary<int, int>> Handle(GetUnreadCountsMesajQuery request, CancellationToken cancellationToken)
        {
            var unreadList = await _mesajRepository.GetListAsync(
                predicate: m => !m.OkunduMu && m.AliciId == request.AliciId && m.StajBasvuruId.HasValue,
                size: 2000,
                cancellationToken: cancellationToken
            );

            return unreadList.Items
                .GroupBy(m => m.StajBasvuruId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
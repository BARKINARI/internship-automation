using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using static Application.Features.StajBasvurulari.Constants.StajBasvuruOperationClaims;

namespace Application.Features.StajBasvurulari.Queries.GetList;

public class GetListStajBasvuruQuery : IRequest<GetListResponse<GetListStajBasvuruListItemDto>>, ISecuredRequest, ICachableRequest, ILoggableRequest
{
    public PageRequest PageRequest { get; set; } = new();

    public string[] Roles => [Admin, Read, StajBasvurular, IletisimOgrenciMesajlari, ProjeTakipNotlandirma, ProjeYonetimi, StajSorumlusu, ProjeYoneticisi];

    public bool BypassCache { get; set; }
    public string CacheKey => $"GetListStajBasvurulari({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string CacheGroupKey => "GetStajBasvurulari";
    public TimeSpan? SlidingExpiration { get; set; }

    public class GetListStajBasvuruQueryHandler : IRequestHandler<GetListStajBasvuruQuery, GetListResponse<GetListStajBasvuruListItemDto>>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IMapper _mapper;

        public GetListStajBasvuruQueryHandler(IStajBasvuruRepository stajBasvuruRepository, IMapper mapper)
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListStajBasvuruListItemDto>> Handle(GetListStajBasvuruQuery request, CancellationToken cancellationToken)
        {
            IPaginate<StajBasvuru> stajBasvurulari = await _stajBasvuruRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListStajBasvuruListItemDto> response = _mapper.Map<GetListResponse<GetListStajBasvuruListItemDto>>(stajBasvurulari);
            return response;
        }
    }
}
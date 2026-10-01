using Application.Features.StajBasvurulari.Queries.GetList;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using static Application.Features.StajBasvurulari.Constants.StajBasvuruOperationClaims;

namespace Application.Features.StajBasvurulari.Queries.GetListByDynamic;

public class GetListByDynamicStajBasvuruQuery : IRequest<GetListResponse<GetListStajBasvuruListItemDto>>, ISecuredRequest, ILoggableRequest
{
    public PageRequest PageRequest { get; set; } = new();
    public DynamicQuery DynamicQuery { get; set; } = new();

    public string[] Roles => [Admin, Read, StajBasvurular, IletisimOgrenciMesajlari, ProjeTakipNotlandirma, ProjeYonetimi, StajSorumlusu, ProjeYoneticisi];

    public class GetListByDynamicStajBasvuruQueryHandler : IRequestHandler<GetListByDynamicStajBasvuruQuery, GetListResponse<GetListStajBasvuruListItemDto>>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IMapper _mapper;

        public GetListByDynamicStajBasvuruQueryHandler(IStajBasvuruRepository stajBasvuruRepository, IMapper mapper)
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListStajBasvuruListItemDto>> Handle(GetListByDynamicStajBasvuruQuery request, CancellationToken cancellationToken)
        {
            IPaginate<StajBasvuru> stajBasvurulari = await _stajBasvuruRepository.GetListByDynamicAsync(
                dynamic: request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListStajBasvuruListItemDto> response = _mapper.Map<GetListResponse<GetListStajBasvuruListItemDto>>(stajBasvurulari);
            return response;
        }
    }
}
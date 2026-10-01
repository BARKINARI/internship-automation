using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Requests;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StajyerProjeler.Queries.GetList;

public class GetListStajyerProjeQuery : IRequest<GetListResponse<GetListStajyerProjeListItemDto>>
{
    public PageRequest PageRequest { get; set; } = new() { PageIndex = 0, PageSize = 10 };

    public class GetListStajyerProjeQueryHandler : IRequestHandler<GetListStajyerProjeQuery, GetListResponse<GetListStajyerProjeListItemDto>>
    {
        private readonly IStajyerProjeRepository _stajyerProjeRepository;
        private readonly IMapper _mapper;

        public GetListStajyerProjeQueryHandler(IStajyerProjeRepository stajyerProjeRepository, IMapper mapper)
        {
            _stajyerProjeRepository = stajyerProjeRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListStajyerProjeListItemDto>> Handle(GetListStajyerProjeQuery request, CancellationToken cancellationToken)
        {
            IPaginate<StajyerProje> stajyerProjeler = await _stajyerProjeRepository.GetListAsync(
                include: q => q.Include(sp => sp.Proje).Include(sp => sp.StajBasvuru),
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListStajyerProjeListItemDto> response = _mapper.Map<GetListResponse<GetListStajyerProjeListItemDto>>(stajyerProjeler);
            return response;
        }
    }
}
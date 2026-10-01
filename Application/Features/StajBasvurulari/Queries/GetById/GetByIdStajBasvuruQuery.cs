using Application.Features.StajBasvurulari.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Domain.Entities;
using MediatR;
using static Application.Features.StajBasvurulari.Constants.StajBasvuruOperationClaims;

namespace Application.Features.StajBasvurulari.Queries.GetById;

public class GetByIdStajBasvuruQuery : IRequest<GetByIdStajBasvuruResponse>, ISecuredRequest, ILoggableRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read, StajBasvurular, StajSorumlusu];

    public class GetByIdStajBasvuruQueryHandler : IRequestHandler<GetByIdStajBasvuruQuery, GetByIdStajBasvuruResponse>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IMapper _mapper;
        private readonly StajBasvuruBusinessRules _stajBasvuruBusinessRules;

        public GetByIdStajBasvuruQueryHandler(
            IStajBasvuruRepository stajBasvuruRepository,
            IMapper mapper,
            StajBasvuruBusinessRules stajBasvuruBusinessRules
        )
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _mapper = mapper;
            _stajBasvuruBusinessRules = stajBasvuruBusinessRules;
        }

        public async Task<GetByIdStajBasvuruResponse> Handle(GetByIdStajBasvuruQuery request, CancellationToken cancellationToken)
        {
            StajBasvuru? stajBasvuru = await _stajBasvuruRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            _stajBasvuruBusinessRules.StajBasvuruShouldExistWhenSelected(stajBasvuru);

            GetByIdStajBasvuruResponse response = _mapper.Map<GetByIdStajBasvuruResponse>(stajBasvuru);
            return response;
        }
    }
}
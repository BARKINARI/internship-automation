using Application.Features.StajBasvurulari.Constants;
using Application.Features.StajBasvurulari.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Domain.Entities;
using MediatR;

namespace Application.Features.StajBasvurulari.Commands.Delete;

public class DeleteStajBasvuruCommand : IRequest<DeletedStajBasvuruResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    public string[] Roles => [StajBasvuruOperationClaims.Admin, StajBasvuruOperationClaims.Write, StajBasvuruOperationClaims.Delete, StajBasvuruOperationClaims.StajBasvurular];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetStajBasvurulari"];

    public class DeleteStajBasvuruCommandHandler : IRequestHandler<DeleteStajBasvuruCommand, DeletedStajBasvuruResponse>
    {
        private readonly IStajBasvuruRepository _stajBasvuruRepository;
        private readonly IMapper _mapper;
        private readonly StajBasvuruBusinessRules _stajBasvuruBusinessRules;

        public DeleteStajBasvuruCommandHandler(
            IStajBasvuruRepository stajBasvuruRepository,
            IMapper mapper,
            StajBasvuruBusinessRules stajBasvuruBusinessRules
        )
        {
            _stajBasvuruRepository = stajBasvuruRepository;
            _mapper = mapper;
            _stajBasvuruBusinessRules = stajBasvuruBusinessRules;
        }

        public async Task<DeletedStajBasvuruResponse> Handle(DeleteStajBasvuruCommand request, CancellationToken cancellationToken)
        {
            StajBasvuru? stajBasvuru = await _stajBasvuruRepository.GetAsync(predicate: s => s.Id == request.Id, cancellationToken: cancellationToken);
            _stajBasvuruBusinessRules.StajBasvuruShouldExistWhenSelected(stajBasvuru);

            await _stajBasvuruRepository.DeleteAsync(stajBasvuru!, cancellationToken: cancellationToken);
            DeletedStajBasvuruResponse response = _mapper.Map<DeletedStajBasvuruResponse>(stajBasvuru);
            return response;
        }
    }
}
using Application.Features.StajyerProjeler.Rules;
using Application.Services.Repositories;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Domain.Entities;
using MediatR;

namespace Application.Features.StajyerProjeler.Commands.DeleteStajyerProje;

public class DeleteStajyerProjeCommand : IRequest<DeletedStajyerProjeResponse>, ITransactionalRequest, ILoggableRequest
{
    public int Id { get; set; }

    public class DeleteStajyerProjeCommandHandler : IRequestHandler<DeleteStajyerProjeCommand, DeletedStajyerProjeResponse>
    {
        private readonly IStajyerProjeRepository _stajyerProjeRepository;
        private readonly StajyerProjeBusinessRules _stajyerProjeBusinessRules;

        public DeleteStajyerProjeCommandHandler(
            IStajyerProjeRepository stajyerProjeRepository,
            StajyerProjeBusinessRules stajyerProjeBusinessRules
        )
        {
            _stajyerProjeRepository = stajyerProjeRepository;
            _stajyerProjeBusinessRules = stajyerProjeBusinessRules;
        }

        public async Task<DeletedStajyerProjeResponse> Handle(DeleteStajyerProjeCommand request, CancellationToken cancellationToken)
        {
            StajyerProje? stajyerProje = await _stajyerProjeRepository.GetAsync(
                predicate: sp => sp.Id == request.Id,
                cancellationToken: cancellationToken
            );
            _stajyerProjeBusinessRules.StajyerProjeShouldExistWhenSelected(stajyerProje);

            await _stajyerProjeRepository.DeleteAsync(stajyerProje!, permanent: true, cancellationToken: cancellationToken);

            return new DeletedStajyerProjeResponse
            {
                Id = request.Id,
                Success = true,
                Message = "Stajyer proje ataması başarıyla kaldırıldı."
            };
        }
    }
}
using Application.Features.Projeler.Rules;
using Application.Services.Repositories;
using Application.Services.StajyerProjeService;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Security.Constants;
using Domain.Entities;
using MediatR;

namespace Application.Features.Projeler.Commands.Delete;

public class DeleteProjeCommand : IRequest<DeletedProjeResponse>, ISecuredRequest, ITransactionalRequest, ILoggableRequest
{
    public int Id { get; set; }

    public string[] Roles => [GeneralOperationClaims.Admin, "Proje.Yonetimi", "ProjeYoneticisi"];

    public class DeleteProjeCommandHandler : IRequestHandler<DeleteProjeCommand, DeletedProjeResponse>
    {
        private readonly IProjeRepository _projeRepository;
        private readonly IStajyerProjeService _stajyerProjeService;
        private readonly ProjeBusinessRules _projeBusinessRules;

        public DeleteProjeCommandHandler(
            IProjeRepository projeRepository,
            IStajyerProjeService stajyerProjeService,
            ProjeBusinessRules projeBusinessRules
        )
        {
            _projeRepository = projeRepository;
            _stajyerProjeService = stajyerProjeService;
            _projeBusinessRules = projeBusinessRules;
        }

        public async Task<DeletedProjeResponse> Handle(DeleteProjeCommand request, CancellationToken cancellationToken)
        {
            Proje? proje = await _projeRepository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);
            _projeBusinessRules.ProjeShouldExistWhenSelected(proje);

            List<StajyerProje> atanmisStajyerProjeler = await _stajyerProjeService.GetByProjeIdAsync(proje!.Id);
            foreach (StajyerProje sp in atanmisStajyerProjeler)
            {
                await _stajyerProjeService.DeleteAsync(sp);
            }

            await _projeRepository.DeleteAsync(proje, cancellationToken: cancellationToken);

            return new DeletedProjeResponse
            {
                Id = proje.Id,
                Baslik = proje.Baslik
            };
        }
    }
}

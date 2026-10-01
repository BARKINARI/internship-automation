using Application.Features.StajyerProjeler.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.StajyerProjeler.Queries.GetById;

public class GetByIdStajyerProjeQuery : IRequest<GetByIdStajyerProjeResponse>
{
    public int Id { get; set; }

    public class GetByIdStajyerProjeQueryHandler : IRequestHandler<GetByIdStajyerProjeQuery, GetByIdStajyerProjeResponse>
    {
        private readonly IStajyerProjeRepository _stajyerProjeRepository;
        private readonly StajyerProjeBusinessRules _stajyerProjeBusinessRules;
        private readonly IMapper _mapper;

        public GetByIdStajyerProjeQueryHandler(
            IStajyerProjeRepository stajyerProjeRepository,
            StajyerProjeBusinessRules stajyerProjeBusinessRules,
            IMapper mapper
        )
        {
            _stajyerProjeRepository = stajyerProjeRepository;
            _stajyerProjeBusinessRules = stajyerProjeBusinessRules;
            _mapper = mapper;
        }

        public async Task<GetByIdStajyerProjeResponse> Handle(GetByIdStajyerProjeQuery request, CancellationToken cancellationToken)
        {
            StajyerProje? stajyerProje = await _stajyerProjeRepository.GetAsync(
                predicate: sp => sp.Id == request.Id,
                include: q => q.Include(sp => sp.Proje).Include(sp => sp.StajBasvuru),
                cancellationToken: cancellationToken
            );

            _stajyerProjeBusinessRules.StajyerProjeShouldExistWhenSelected(stajyerProje);

            GetByIdStajyerProjeResponse response = _mapper.Map<GetByIdStajyerProjeResponse>(stajyerProje);
            if (stajyerProje!.Proje != null) response.ProjeBaslik = stajyerProje.Proje.Baslik;
            if (stajyerProje!.StajBasvuru != null) response.StajyerAdSoyad = $"{stajyerProje.StajBasvuru.Ad} {stajyerProje.StajBasvuru.Soyad}";

            return response;
        }
    }
}
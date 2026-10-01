using Application.Features.Projeler.Commands.Create;
using Application.Features.Projeler.Commands.Update;
using Application.Features.Projeler.Queries.GetById;
using Application.Features.Projeler.Queries.GetList;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Projeler.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Proje, CreateProjeCommand>().ReverseMap();
        CreateMap<Proje, CreatedProjeResponse>().ReverseMap();
        CreateMap<Proje, UpdateProjeCommand>().ReverseMap();
        CreateMap<Proje, UpdatedProjeResponse>().ReverseMap();
        CreateMap<Proje, GetListProjeListItemDto>().ReverseMap();
        CreateMap<Proje, GetByIdProjeResponse>().ReverseMap();
    }
}

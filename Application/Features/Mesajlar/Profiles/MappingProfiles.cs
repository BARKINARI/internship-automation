using Application.Features.Mesajlar.Commands.Create;
using Application.Features.Mesajlar.Queries.GetListByStajBasvuruId;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Mesajlar.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Mesaj, CreateMesajCommand>().ReverseMap();
        CreateMap<Mesaj, CreatedMesajResponse>().ReverseMap();
        CreateMap<Mesaj, GetListByStajBasvuruIdMesajListItemDto>().ReverseMap();
    }
}

using Application.Features.StajyerProjeler.Commands.AssignStajyer;
using Application.Features.StajyerProjeler.Commands.DeleteStajyerProje;
using Application.Features.StajyerProjeler.Commands.UpdateStajyerProje;
using Application.Features.StajyerProjeler.Queries.GetById;
using Application.Features.StajyerProjeler.Queries.GetList;
using Application.Features.StajyerProjeler.Queries.GetStajyerProjeleri;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Domain.Entities;

namespace Application.Features.StajyerProjeler.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<StajyerProje, AssignedStajyerProjeResponse>().ReverseMap();
        CreateMap<StajyerProje, DeletedStajyerProjeResponse>().ReverseMap();
        CreateMap<StajyerProje, GetByIdStajyerProjeResponse>()
            .ForMember(dest => dest.ProjeBaslik, opt => opt.MapFrom(src => src.Proje != null ? src.Proje.Baslik : ""))
            .ForMember(dest => dest.StajyerAdSoyad, opt => opt.MapFrom(src => src.StajBasvuru != null ? $"{src.StajBasvuru.Ad} {src.StajBasvuru.Soyad}" : ""))
            .ReverseMap();
        CreateMap<StajyerProje, GetListStajyerProjeListItemDto>()
            .ForMember(dest => dest.ProjeBaslik, opt => opt.MapFrom(src => src.Proje != null ? src.Proje.Baslik : ""))
            .ForMember(dest => dest.StajyerAdSoyad, opt => opt.MapFrom(src => src.StajBasvuru != null ? $"{src.StajBasvuru.Ad} {src.StajBasvuru.Soyad}" : ""))
            .ReverseMap();
        CreateMap<IPaginate<StajyerProje>, GetListResponse<GetListStajyerProjeListItemDto>>().ReverseMap();
    }
}
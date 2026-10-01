using Application.Features.StajBasvurulari.Commands.Create;
using Application.Features.StajBasvurulari.Commands.Delete;
using Application.Features.StajBasvurulari.Commands.Update;
using Application.Features.StajBasvurulari.Queries.GetById;
using Application.Features.StajBasvurulari.Queries.GetList;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Domain.Entities;

namespace Application.Features.StajBasvurulari.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<StajBasvuru, CreateStajBasvuruCommand>().ReverseMap();
        CreateMap<StajBasvuru, CreatedStajBasvuruResponse>().ReverseMap();
        CreateMap<StajBasvuru, UpdateStajBasvuruCommand>().ReverseMap();
        CreateMap<StajBasvuru, UpdatedStajBasvuruResponse>().ReverseMap();
        CreateMap<StajBasvuru, DeleteStajBasvuruCommand>().ReverseMap();
        CreateMap<StajBasvuru, DeletedStajBasvuruResponse>().ReverseMap();
        CreateMap<StajBasvuru, GetByIdStajBasvuruResponse>().ReverseMap();
        CreateMap<StajBasvuru, GetListStajBasvuruListItemDto>().ReverseMap();
        CreateMap<IPaginate<StajBasvuru>, GetListResponse<GetListStajBasvuruListItemDto>>().ReverseMap();
    }
}
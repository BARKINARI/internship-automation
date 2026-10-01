using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.Queries.GetById;
using Application.Features.Users.Queries.GetList;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Core.Security.Entities;

namespace Application.Features.Users.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User<int>, CreateUserCommand>().ReverseMap();
        CreateMap<User<int>, CreatedUserResponse>().ReverseMap();
        CreateMap<User<int>, UpdateUserCommand>().ReverseMap();
        CreateMap<User<int>, UpdatedUserResponse>().ReverseMap();
        CreateMap<User<int>, DeleteUserCommand>().ReverseMap();
        CreateMap<User<int>, DeletedUserResponse>().ReverseMap();
        CreateMap<User<int>, GetByIdUserResponse>().ReverseMap();
        CreateMap<User<int>, GetListUserListItemDto>().ReverseMap();
        CreateMap<IPaginate<User<int>>, GetListResponse<GetListUserListItemDto>>().ReverseMap();
    }
}
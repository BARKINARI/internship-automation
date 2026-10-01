using Application.Features.UserOperationClaims.Commands.Create;
using Application.Features.UserOperationClaims.Commands.Delete;
using Application.Features.UserOperationClaims.Commands.Update;
using Application.Features.UserOperationClaims.Queries.GetById;
using Application.Features.UserOperationClaims.Queries.GetList;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Core.Security.Entities;

namespace Application.Features.UserOperationClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<UserOperationClaim<int, int, int>, CreateUserOperationClaimCommand>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, CreatedUserOperationClaimResponse>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, UpdateUserOperationClaimCommand>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, UpdatedUserOperationClaimResponse>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, DeleteUserOperationClaimCommand>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, DeletedUserOperationClaimResponse>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, GetByIdUserOperationClaimResponse>().ReverseMap();
        CreateMap<UserOperationClaim<int, int, int>, GetListUserOperationClaimListItemDto>().ReverseMap();
        CreateMap<IPaginate<UserOperationClaim<int, int, int>>, GetListResponse<GetListUserOperationClaimListItemDto>>().ReverseMap();
    }
}
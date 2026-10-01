using Application.Features.OperationClaims.Commands.Create;
using Application.Features.OperationClaims.Commands.Delete;
using Application.Features.OperationClaims.Commands.Update;
using Application.Features.OperationClaims.Queries.GetById;
using Application.Features.OperationClaims.Queries.GetList;
using AutoMapper;
using Core.Application.Responses;
using Core.Persistence.Paging;
using Core.Security.Entities;

namespace Application.Features.OperationClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<OperationClaim<int>, CreateOperationClaimCommand>().ReverseMap();
        CreateMap<OperationClaim<int>, CreatedOperationClaimResponse>().ReverseMap();
        CreateMap<OperationClaim<int>, UpdateOperationClaimCommand>().ReverseMap();
        CreateMap<OperationClaim<int>, UpdatedOperationClaimResponse>().ReverseMap();
        CreateMap<OperationClaim<int>, DeleteOperationClaimCommand>().ReverseMap();
        CreateMap<OperationClaim<int>, DeletedOperationClaimResponse>().ReverseMap();
        CreateMap<OperationClaim<int>, GetByIdOperationClaimResponse>().ReverseMap();
        CreateMap<OperationClaim<int>, GetListOperationClaimListItemDto>().ReverseMap();
        CreateMap<IPaginate<OperationClaim<int>>, GetListResponse<GetListOperationClaimListItemDto>>().ReverseMap();
    }
}
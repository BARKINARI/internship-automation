using Application.Services.Repositories;
using Application.Services.StajBasvuruService;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Security.Constants;
using Core.Security.Entities;
using MediatR;

namespace Application.Features.Users.Queries.GetListWithRoles;

public class GetListUserWithRolesQuery : IRequest<List<GetListUserWithRolesListItemDto>>, ISecuredRequest, ILoggableRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, "Users.Read", "Users.Admin"];

    public class GetListUserWithRolesQueryHandler : IRequestHandler<GetListUserWithRolesQuery, List<GetListUserWithRolesListItemDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IOperationClaimRepository _operationClaimRepository;
        private readonly IStajBasvuruService _stajBasvuruService;

        public GetListUserWithRolesQueryHandler(
            IUserRepository userRepository,
            IUserOperationClaimRepository userOperationClaimRepository,
            IOperationClaimRepository operationClaimRepository,
            IStajBasvuruService stajBasvuruService)
        {
            _userRepository = userRepository;
            _userOperationClaimRepository = userOperationClaimRepository;
            _operationClaimRepository = operationClaimRepository;
            _stajBasvuruService = stajBasvuruService;
        }

        public async Task<List<GetListUserWithRolesListItemDto>> Handle(GetListUserWithRolesQuery request, CancellationToken cancellationToken)
        {
            var usersPaginate = await _userRepository.GetListAsync(
                size: 1000,
                orderBy: q => q.OrderBy(u => u.Id),
                cancellationToken: cancellationToken
            );

            var claimsPaginate = await _operationClaimRepository.GetListAsync(size: 1000, cancellationToken: cancellationToken);
            var userClaimsPaginate = await _userOperationClaimRepository.GetListAsync(size: 2000, cancellationToken: cancellationToken);

            var claimsDict = claimsPaginate.Items.ToDictionary(c => c.Id, c => c.Name);

            var basvurularPaginate = await _stajBasvuruService.GetListAsync(
                predicate: b => b.UserId != null,
                size: 1000,
                cancellationToken: cancellationToken
            );
            var basvuruDict = (basvurularPaginate?.Items ?? [])
                .Where(b => b.UserId.HasValue)
                .GroupBy(b => b.UserId!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            var result = new List<GetListUserWithRolesListItemDto>();

            foreach (var user in usersPaginate.Items)
            {
                var userRoles = userClaimsPaginate.Items
                    .Where(uc => uc.UserId == user.Id && claimsDict.ContainsKey(uc.OperationClaimId))
                    .Select(uc => new UserRoleItemDto
                    {
                        UserOperationClaimId = uc.Id,
                        OperationClaimId = uc.OperationClaimId,
                        OperationClaimName = claimsDict[uc.OperationClaimId]
                    })
                    .ToList();

                basvuruDict.TryGetValue(user.Id, out var basvuru);

                result.Add(new GetListUserWithRolesListItemDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Status = user.Status,
                    CreatedDate = user.CreatedDate,
                    Roles = userRoles,
                    StajBasvuruId = basvuru?.Id,
                    Okul = basvuru?.Okul,
                    Bolum = basvuru?.Bolum,
                    StajKonusu = basvuru?.StajKonusu
                });
            }

            return result;
        }
    }
}

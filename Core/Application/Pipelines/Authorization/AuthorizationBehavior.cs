using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Core.CrossCuttingConcerns.Exception.Types;
using Core.Security.Constants;
using Core.Security.Extensions;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_httpContextAccessor.HttpContext.User.Claims.Any())
            throw new AuthorizationException("Giriş yapmadınız. Lütfen önce oturum açınız.");

        if (request.Roles.Any())
        {
            ICollection<string>? userRoleClaims = _httpContextAccessor.HttpContext.User.GetRoleClaims() ?? [];
            bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoleClaims
                .FirstOrDefault(userRoleClaim =>
                    userRoleClaim == GeneralOperationClaims.Admin || request.Roles.Contains(userRoleClaim)
                )
                == null;
            if (isNotMatchedAUserRoleClaimWithRequestRoles)
                throw new AuthorizationException("Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.");
        }

        TResponse response = await next();
        return response;
    }
}

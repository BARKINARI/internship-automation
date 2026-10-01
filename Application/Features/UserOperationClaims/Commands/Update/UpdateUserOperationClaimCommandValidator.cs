using FluentValidation;

namespace Application.Features.UserOperationClaims.Commands.Update;

public class UpdateUserOperationClaimCommandValidator : AbstractValidator<UpdateUserOperationClaimCommand>
{
    public UpdateUserOperationClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().GreaterThan(0);
        RuleFor(c => c.UserId).NotEmpty().GreaterThan(0);
        RuleFor(c => c.OperationClaimId).NotEmpty().GreaterThan(0);
    }
}
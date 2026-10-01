using FluentValidation;

namespace Application.Features.UserOperationClaims.Commands.Create;

public class CreateUserOperationClaimCommandValidator : AbstractValidator<CreateUserOperationClaimCommand>
{
    public CreateUserOperationClaimCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().GreaterThan(0);
        RuleFor(c => c.OperationClaimId).NotEmpty().GreaterThan(0);
    }
}
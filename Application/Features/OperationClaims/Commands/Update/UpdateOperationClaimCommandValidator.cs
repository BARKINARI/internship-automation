using FluentValidation;

namespace Application.Features.OperationClaims.Commands.Update;

public class UpdateOperationClaimCommandValidator : AbstractValidator<UpdateOperationClaimCommand>
{
    public UpdateOperationClaimCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty().GreaterThan(0);
        RuleFor(c => c.Name).NotEmpty().MinimumLength(2);
    }
}